using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MethodDecorator.Fody;

using Mono.Cecil;
using MethodDecorator.Fody.Interfaces;
using System.Text.RegularExpressions;
using Mono.Collections.Generic;

public class ModuleWeaver {
    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }
    public Action<string> LogError { get; set; }


    public void Execute() {
        this.LogInfo = s => { };
        this.LogWarning = s => { };

        var decorator = new MethodDecorator.Fody.MethodDecorator(this.ModuleDefinition);

        foreach (var x in this.ModuleDefinition.AssemblyReferences) AssemblyResolver.Resolve(x);

        this.DecorateAttributedByImplication(decorator);
		this.DecorateByType(decorator);
    }

	private void DecorateByType(MethodDecorator.Fody.MethodDecorator decorator)
	{
		var referenceFinder = new ReferenceFinder(this.ModuleDefinition);
		var markerTypeDefinitions = this.FindMarkerTypes();

		// Look for rules in the assembly and module.
		var assemblyRules = FindAspectRules(this.ModuleDefinition.Assembly.CustomAttributes);
		var moduleRules = FindAspectRules(this.ModuleDefinition.CustomAttributes);
			
		// Read the top-level and nested types from this module
		foreach(var type in this.ModuleDefinition.Types.SelectMany(x => GetAllTypes(x)))
		{
			// Look for rules on the type and marker attributes
			var classRules = FindByMarkerType(markerTypeDefinitions, type.CustomAttributes)
				.Concat(FindAspectRules(type.CustomAttributes, true));

			// Loop through all methods in this type
			foreach(var method in type.Methods.Where(x => x.HasBody))
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
					.Select((Rule, ScopeOrdering) => new { Rule, ScopeOrdering });

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
                                .OrderBy( x => x.Rule.AttributePriority)
                                .ThenByDescending(x => x.ScopeOrdering)
                                .Select( x => x.Rule)
                                .FirstOrDefault();

					// If we have a rule and it isn't an exclusion, apply the method decoration.
					if( rule!=null && !rule.AttributeExclude)
					{
						decorator.Decorate(
							type,
							method,
							rule.MethodDecoratorAttribute);
					}
				}
			}
		}
	}

	private IEnumerable<AspectRule> FindByMarkerType(
		IEnumerable<TypeDefinition> markerTypeDefinitions,
		Collection<CustomAttribute> customAttributes)
	{
		foreach(var attr in customAttributes)
		{
			var attributeTypeDef = attr.AttributeType.Resolve();

			foreach(var markerTypeDefinition in markerTypeDefinitions)
			{
				if(attributeTypeDef.Implements(markerTypeDefinition)
					|| attributeTypeDef.DerivesFrom(markerTypeDefinition)
					|| this.AreEquals(attributeTypeDef, markerTypeDefinition))
				{
					yield return new AspectRule()
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

	private IEnumerable<AttributeMethodInfo> FindAttributedMethods(IEnumerable<TypeDefinition> markerTypeDefintions)
	{
		return from topLevelType in this.ModuleDefinition.Types
			   from type in GetAllTypes(topLevelType)
			   from method in type.Methods
			   where method.HasBody
			   from attribute in method.CustomAttributes.Concat(method.DeclaringType.CustomAttributes)
			   let attributeTypeDef = attribute.AttributeType.Resolve()
			   from markerTypeDefinition in markerTypeDefintions
			   where attributeTypeDef.Implements(markerTypeDefinition) ||
					 attributeTypeDef.DerivesFrom(markerTypeDefinition) ||
					 this.AreEquals(attributeTypeDef, markerTypeDefinition)
			   select new AttributeMethodInfo
			   {
				   CustomAttribute = attribute,
				   TypeDefinition = type,
				   MethodDefinition = method
			   };
	}

	private IEnumerable<TypeDefinition> FindMarkerTypes()
	{
		var allAttributes = this.GetAttributes();

		var markerTypeDefinitions = (from type in allAttributes
									 where HasCorrectMethods(type) 
									 && !type.Implements(typeof(IAspectMatchingRule))
									 select type).ToList();

		//if(!markerTypeDefinitions.Any())
		//{
		//	if(null != LogError)
		//		LogError("Could not find any method decorator attribute");
		//	throw new WeavingException("Could not find any method decorator attribute");
		//}

		return markerTypeDefinitions;
	}


	private IEnumerable<AspectRule> FindAspectRules(
		IEnumerable<CustomAttribute> attrs,
		bool explicitMatch = false)
	{
		return attrs
			.Where(attr => IsAspectMatchingRule(attr))
			.Select(attr => new AspectRule()
		{
			AttributeTargetTypes = GetAttributeProperty<string>(attr, "AttributeTargetTypes"),
			AttributeExclude = GetAttributeProperty<bool>(attr, "AttributeExclude"),
			AttributePriority = GetAttributeProperty<int>(attr, "AttributePriority"),
            AspectPriority = GetAttributeProperty<int>(attr, "AspectPriority"),
            MethodDecoratorAttribute = attr,
			ExplicitMatch = explicitMatch
		});
	}

	private T GetAttributeProperty<T>(CustomAttribute attr, string propertyName)
	{
		if(!attr.Properties.Any(x => x.Name == propertyName))
			return default(T);

		return (T)attr.Properties.First(x => x.Name == propertyName).Argument.Value;
	}

	private bool IsAspectMatchingRule(CustomAttribute x)
	{
		var typeDefinition = x.AttributeType.Resolve();

		// Avoid problem on initial load of types where mscorlib not loaded - the Implements()
		// method crashes if this happens.
		if(!typeDefinition.Module.AssemblyReferences.Any(a => a.Name == "mscorlib"))
			return false;

		return typeDefinition.Implements(typeof(IAspectMatchingRule));
	}

	private void DecorateAttributedByImplication(MethodDecorator.Fody.MethodDecorator decorator) {
        var inderectAttributes = this.ModuleDefinition.CustomAttributes
                                     .Concat(this.ModuleDefinition.Assembly.CustomAttributes)
                                     .Where(x => x.AttributeType.Name.StartsWith("IntersectMethodsMarkedByAttribute"))
                                     .Select(ToHostAttributeMapping)
                                     .Where(x=>x!=null)
                                     .ToArray();

        foreach (var inderectAttribute in inderectAttributes) {
            var methods = this.FindAttributedMethods(inderectAttribute.AttribyteTypes);
            foreach (var x in methods)
                decorator.Decorate(x.TypeDefinition, x.MethodDefinition, inderectAttribute.HostAttribute);
        }
    }

    private HostAttributeMapping ToHostAttributeMapping(CustomAttribute arg) {
        var prms = arg.ConstructorArguments.First().Value as CustomAttributeArgument[];
        if (null == prms)
            return null;
        return new HostAttributeMapping {
            HostAttribute = arg,
            AttribyteTypes = prms.Select(c => ((TypeReference)c.Value).Resolve()).ToArray()
        };
    }




    private IEnumerable<TypeDefinition> GetAttributes() {
        
        var res = new List<TypeDefinition>();

        res.AddRange(this.ModuleDefinition.CustomAttributes.Select(c => c.AttributeType.Resolve()));
        res.AddRange(this.ModuleDefinition.Assembly.CustomAttributes.Select(c => c.AttributeType.Resolve()));

        if (this.ModuleDefinition.Runtime >= TargetRuntime.Net_4_0) {
            //will find if assembly is loaded
            var methodDecorator = Type.GetType("MethodDecorator.Fody.Interfaces.IMethodDecorator, MethodDecoratorInterfaces");

            //make using of MethodDecoratorEx assembly optional because it can break exists code
            if (null != methodDecorator) {
                
                res.AddRange(this.ModuleDefinition.Types.Where(c => c.Implements(methodDecorator)));
            }
        }

        return res;
    }

    private static bool HasCorrectMethods(TypeDefinition type) {
        return type.Methods.Any(IsOnEntryMethod) &&
               type.Methods.Any(IsOnExitMethod) &&
               type.Methods.Any(IsOnExceptionMethod);
    }

    private static bool IsOnEntryMethod(MethodDefinition m) {
        return m.Name == "OnEntry" &&
               m.Parameters.Count == 0;
    }

    private static bool IsOnExitMethod(MethodDefinition m) {
        return m.Name == "OnExit" &&
               m.Parameters.Count == 0;
    }

    private static bool IsOnExceptionMethod(MethodDefinition m) {
        return m.Name == "OnException" && m.Parameters.Count == 1 &&
               m.Parameters[0].ParameterType.FullName == typeof(Exception).FullName;
    }

    private static bool IsOnTaskContinuationMethod(MethodDefinition m) {
        return m.Name == "OnTaskContinuation" && m.Parameters.Count == 1
            && m.Parameters[0].ParameterType.FullName == typeof(Task).FullName;
    }



    private bool AreEquals(TypeDefinition attributeTypeDef, TypeDefinition markerTypeDefinition) {
        return attributeTypeDef.FullName == markerTypeDefinition.FullName;
    }

    private static IEnumerable<TypeDefinition> GetAllTypes(TypeDefinition type) {
        yield return type;

        var allNestedTypes = from t in type.NestedTypes
                             from t2 in GetAllTypes(t)
                             select t2;

        foreach (var t in allNestedTypes)
            yield return t;
    }

    private class HostAttributeMapping {
        public TypeDefinition[] AttribyteTypes { get; set; }
        public CustomAttribute HostAttribute { get; set; }
    }

    private class AttributeMethodInfo {
        public TypeDefinition TypeDefinition { get; set; }
        public MethodDefinition MethodDefinition { get; set; }
        public CustomAttribute CustomAttribute { get; set; }
    }

	private class AspectRule {

		private const string _regexPrefix = "regex:";

		private string _attributeTargetTypes;
		private Regex _matchRegex;

		public string AttributeTargetTypes
		{
			get { return _attributeTargetTypes; }
			set
			{
				_attributeTargetTypes = value;

				if(value != null)
				{
					string pattern;
					if(value.StartsWith(_regexPrefix))
					{
						pattern = value.Substring(_regexPrefix.Length);
					}
					else
					{
						pattern = String.Join("|",			// "OR" each comma-separated item
							value.Split(new[] { ',' })		// (split by comma)
								.Select(x => x.Trim(" \t\r\n".ToCharArray()))
								.Select(t => 
									"^"						// Anchor to start
									+ String.Join(".*",			// Convert * to .*
										t.Split(new[] { '*' })
										.Select(x => Regex.Escape(x)))	// Convert '.' into '\.'
									+ "$"));				// Anchor to end
					}

					_matchRegex = new Regex(pattern);
				}
				else
				{
					_matchRegex = null;
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
			if(this.AttributeTargetTypes == null)
				return this.ExplicitMatch;

			string completeMethodName = type.Namespace + "." + type.Name + "." + method.Name;

			var result = _matchRegex.IsMatch(completeMethodName);
			return result;
		}
	}
}
