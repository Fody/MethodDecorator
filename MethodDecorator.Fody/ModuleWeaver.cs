using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mono.Cecil;
using System.Text.RegularExpressions;
using Fody;
using Mono.Collections.Generic;

public partial class ModuleWeaver : BaseModuleWeaver
{
    TypeReference objectTypeRef;
    TypeReference methodBaseTypeRef;
    TypeReference exceptionTypeRef;
    TypeDefinition systemTypeRef;
    TypeDefinition memberInfoRef;
    TypeDefinition activatorTypeRef;
    TypeDefinition attributeTypeRef;

    public override void Execute()
    {
        methodBaseTypeRef = ModuleDefinition.ImportReference(FindTypeDefinition("System.Reflection.MethodBase"));
        exceptionTypeRef = ModuleDefinition.ImportReference(FindTypeDefinition("System.Exception"));
        objectTypeRef = ModuleDefinition.ImportReference(TypeSystem.ObjectDefinition);
        systemTypeRef = FindTypeDefinition("System.Type");
        memberInfoRef = FindTypeDefinition("System.Reflection.MemberInfo");
        activatorTypeRef = FindTypeDefinition("System.Activator");
        attributeTypeRef = FindTypeDefinition("System.Attribute");
        referenceFinder = new ReferenceFinder(ModuleDefinition);

        DecorateAttributedByImplication();
        DecorateByType();
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "netstandard";
        yield return "mscorlib";
        yield return "System";
        yield return "System.Runtime";
        yield return "System.Core";
    }

    void DecorateByType()
    {
        var markerTypeDefinitions = FindMarkerTypes().ToList();

        // Look for rules in the assembly and module.
        var assemblyRules = FindAspectRules(ModuleDefinition.Assembly.CustomAttributes).ToList();
        var moduleRules = FindAspectRules(ModuleDefinition.CustomAttributes).ToList();

        // Read the top-level and nested types from this module
        foreach (var type in ModuleDefinition.Types.SelectMany(GetAllTypes))
        {
            // Look for rules on the type and marker attributes
            var classRules = FindByMarkerType(markerTypeDefinitions, type.CustomAttributes)
                .Concat(FindAspectRules(type.CustomAttributes, true))
                .ToList();

            // Loop through all methods in this type
            foreach (var method in type.Methods.Where(x => x.HasBody))
            {
                // Find any rules applied to the method.
                var methodRules = FindByMarkerType(markerTypeDefinitions, method.CustomAttributes)
                    .Concat(FindAspectRules(method.CustomAttributes, true));

                // Join together all the rules and give them an ordering number starting at 0 for
                // the highest level (assembly) to N as a lowest level (last attribute on the method)
                var allRules = assemblyRules
                    .Concat(moduleRules)
                    .Concat(classRules)
                    .Concat(methodRules)
                    .Select((Rule, ScopeOrdering) => new {Rule, ScopeOrdering});

                var orderedList = allRules
                    .Where(x => x.Rule.Match(type, method))
                    .OrderByDescending(x => x.Rule.AspectPriority)
                    .ThenByDescending(x => x.ScopeOrdering)
                    .GroupBy(x => x.Rule.MethodDecoratorAttribute.AttributeType);

                // Group the rules by the aspect type
                foreach (var aspectSet in orderedList)
                {
                    // Sort the rules in priority order (so that attributes applied to the
                    // method take precedence over the type, module then assembly)
                    // Then pick out the first rule - this tells us whether to include
                    // or exclude.

                    var rule = aspectSet
                        .OrderBy(x => x.Rule.AttributePriority)
                        .ThenByDescending(x => x.ScopeOrdering)
                        .Select(x => x.Rule)
                        .FirstOrDefault();

                    // If we have a rule and it isn't an exclusion, apply the method decoration.
                    if (rule != null && !rule.AttributeExclude)
                    {
                        Decorate(
                            type,
                            method,
                            rule.MethodDecoratorAttribute,
                            rule.ExplicitMatch);
                    }
                }
            }
        }
    }

    IEnumerable<AspectRule> FindByMarkerType(
        List<TypeDefinition> markerTypeDefinitions,
        Collection<CustomAttribute> customAttributes)
    {
        foreach (var attr in customAttributes)
        {
            var attributeTypeDef = attr.AttributeType.Resolve();

            foreach (var markerTypeDefinition in markerTypeDefinitions)
            {
                if (attributeTypeDef.Implements(markerTypeDefinition)
                    || attributeTypeDef.DerivesFrom(markerTypeDefinition)
                    || AreEquals(attributeTypeDef, markerTypeDefinition))
                {
                    yield return new AspectRule
                    {
                        MethodDecoratorAttribute = attr,
                        AttributeExclude = false,
                        AttributePriority = 0,
                        AspectPriority = 0,
                        ExplicitMatch = true
                    };
                }
            }
        }
    }

    IEnumerable<AttributeMethodInfo> FindAttributedMethods(IEnumerable<TypeDefinition> markerTypeDefinitions)
    {
        return from topLevelType in ModuleDefinition.Types
            from type in GetAllTypes(topLevelType)
            from method in type.Methods
            where method.HasBody
            from attribute in method.CustomAttributes.Concat(method.DeclaringType.CustomAttributes)
            let attributeTypeDef = attribute.AttributeType.Resolve()
            from markerTypeDefinition in markerTypeDefinitions
            where attributeTypeDef.Implements(markerTypeDefinition) ||
                  attributeTypeDef.DerivesFrom(markerTypeDefinition) ||
                  AreEquals(attributeTypeDef, markerTypeDefinition)
            select new AttributeMethodInfo
            {
                CustomAttribute = attribute,
                TypeDefinition = type,
                MethodDefinition = method
            };
    }

    IEnumerable<TypeDefinition> FindMarkerTypes()
    {
        var allAttributes = GetAttributes();

        return from type in allAttributes
            where HasCorrectMethods(type)
                  && !type.Implements("MethodDecorator.Fody.Interfaces.IAspectMatchingRule")
            select type;
    }

    IEnumerable<AspectRule> FindAspectRules(
        IEnumerable<CustomAttribute> attrs,
        bool explicitMatch = false)
    {
        return attrs
            .Where(IsAspectMatchingRule)
            .Select(attr => new AspectRule
            {
                AttributeTargetTypes = GetAttributeProperty<string>(attr, "AttributeTargetTypes"),
                AttributeExclude = GetAttributeProperty<bool>(attr, "AttributeExclude"),
                AttributePriority = GetAttributeProperty<int>(attr, "AttributePriority"),
                AspectPriority = GetAttributeProperty<int>(attr, "AspectPriority"),
                MethodDecoratorAttribute = attr,
                ExplicitMatch = explicitMatch
            });
    }

    T GetAttributeProperty<T>(CustomAttribute attr, string propertyName)
    {
        if (attr.Properties.All(x => x.Name != propertyName))
            return default;

        return (T) attr.Properties.First(x => x.Name == propertyName).Argument.Value;
    }

    bool IsAspectMatchingRule(CustomAttribute x)
    {
        var typeDefinition = x.AttributeType.Resolve();

        return typeDefinition.Implements("MethodDecorator.Fody.Interfaces.IAspectMatchingRule");
    }

    void DecorateAttributedByImplication()
    {
        var indirectAttributes = ModuleDefinition.CustomAttributes
            .Concat(ModuleDefinition.Assembly.CustomAttributes)
            .Where(x => x.AttributeType.Name.StartsWith("IntersectMethodsMarkedByAttribute"))
            .Select(ToHostAttributeMapping)
            .Where(x => x != null)
            .ToArray();

        foreach (var indirectAttribute in indirectAttributes)
        {
            var methods = FindAttributedMethods(indirectAttribute.AttributeTypes);
            foreach (var x in methods)
                Decorate(x.TypeDefinition, x.MethodDefinition, indirectAttribute.HostAttribute, false);
        }
    }

    HostAttributeMapping ToHostAttributeMapping(CustomAttribute arg)
    {
        if (!(arg.ConstructorArguments.First().Value is CustomAttributeArgument[] arguments))
            return null;
        return new HostAttributeMapping
        {
            HostAttribute = arg,
            AttributeTypes = arguments.Select(c => ((TypeReference) c.Value).Resolve()).ToArray()
        };
    }

    IEnumerable<TypeDefinition> GetAttributes()
    {
        var res = new List<TypeDefinition>();

        res.AddRange(ModuleDefinition.CustomAttributes.Select(c => c.AttributeType.Resolve()));
        res.AddRange(ModuleDefinition.Assembly.CustomAttributes.Select(c => c.AttributeType.Resolve()));

        if (ModuleDefinition.Runtime >= TargetRuntime.Net_4_0)
        {
            res.AddRange(ModuleDefinition.Types.Where(c => c.Implements("MethodDecorator.Fody.Interfaces.IMethodDecorator")));
        }

        return res;
    }

    static bool HasCorrectMethods(TypeDefinition type)
    {
        return type.Methods.Any(IsOnEntryMethod) &&
               type.Methods.Any(IsOnExitMethod) &&
               type.Methods.Any(IsOnExceptionMethod);
    }

    static bool IsOnEntryMethod(MethodDefinition m)
    {
        return m.Name == "OnEntry" &&
               m.Parameters.Count == 0;
    }

    static bool IsOnExitMethod(MethodDefinition m)
    {
        return m.Name == "OnExit" &&
               m.Parameters.Count == 0;
    }

    static bool IsOnExceptionMethod(MethodDefinition m)
    {
        return m.Name == "OnException" && m.Parameters.Count == 1 &&
               m.Parameters[0].ParameterType.FullName == typeof(Exception).FullName;
    }

    //TODO
    static bool IsOnTaskContinuationMethod(MethodDefinition m)
    {
        return m.Name == "OnTaskContinuation" && m.Parameters.Count == 1
                                              && m.Parameters[0].ParameterType.FullName == typeof(Task).FullName;
    }

    bool AreEquals(TypeDefinition attributeTypeDef, TypeDefinition markerTypeDefinition)
    {
        return attributeTypeDef.FullName == markerTypeDefinition.FullName;
    }

    static IEnumerable<TypeDefinition> GetAllTypes(TypeDefinition type)
    {
        yield return type;

        var allNestedTypes = from t in type.NestedTypes
            from t2 in GetAllTypes(t)
            select t2;

        foreach (var t in allNestedTypes)
            yield return t;
    }

    class HostAttributeMapping
    {
        public TypeDefinition[] AttributeTypes { get; set; }
        public CustomAttribute HostAttribute { get; set; }
    }

    class AttributeMethodInfo
    {
        public TypeDefinition TypeDefinition { get; set; }
        public MethodDefinition MethodDefinition { get; set; }
        public CustomAttribute CustomAttribute { get; set; }
    }

    class AspectRule
    {
        const string regexPrefix = "regex:";

        string attributeTargetTypes;
        Regex matchRegex;

        public string AttributeTargetTypes
        {
            get { return attributeTargetTypes; }
            set
            {
                attributeTargetTypes = value;

                if (value != null)
                {
                    string pattern;
                    if (value.StartsWith(regexPrefix))
                    {
                        pattern = value.Substring(regexPrefix.Length);
                    }
                    else
                    {
                        pattern = string.Join("|", // "OR" each comma-separated item
                            value.Split(',') // (split by comma)
                                .Select(x => x.Trim(" \t\r\n".ToCharArray()))
                                .Select(t =>
                                    "^" // Anchor to start
                                    + string.Join(".*", // Convert * to .*
                                        t.Split('*')
                                            .Select(Regex.Escape)) // Convert '.' into '\.'
                                    + "$")); // Anchor to end
                    }

                    matchRegex = new Regex(pattern);
                }
                else
                {
                    matchRegex = null;
                }
            }
        }

        public bool AttributeExclude { get; set; }
        public int AttributePriority { get; set; }
        public int AspectPriority { get; set; }
        public CustomAttribute MethodDecoratorAttribute { get; set; }
        public bool ExplicitMatch { get; internal set; }

        internal bool Match(TypeDefinition type, MethodDefinition method)
        {
            if (AttributeTargetTypes == null)
            {
                return ExplicitMatch;
            }

            var completeMethodName = $"{type.Namespace}.{type.Name}.{method.Name}";

            return matchRegex.IsMatch(completeMethodName);
        }
    }
}