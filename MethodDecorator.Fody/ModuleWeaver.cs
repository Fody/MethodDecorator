using System;
using System.Collections.Generic;
using System.Linq;
using MethodDecoratorEx.Fody;
using Mono.Cecil;

public class ModuleWeaver
{
    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }
    public Action<string> LogError { get; set; }

    public void Execute()
    {
        LogInfo = s => { };
        LogWarning = s => { };

        var decorator = new MethodDecoratorEx.Fody.MethodDecorator(ModuleDefinition);

        foreach (var x in ModuleDefinition.AssemblyReferences) AssemblyResolver.Resolve(x);

        DecorateDirectlyAttributed(decorator);
        DecorateAttributedByImplication(decorator);
    }

    private void DecorateAttributedByImplication(MethodDecoratorEx.Fody.MethodDecorator decorator)
    {
        var inderectAttributes = ModuleDefinition.CustomAttributes
            .Concat(ModuleDefinition.Assembly.CustomAttributes)
            .Where(x => x.AttributeType.Name.StartsWith("IntersectMethodsMarkedByAttribute"))
            .Select(ToHostAttributeMapping)
            .Where(x => x != null)
            .ToArray();

        foreach (var inderectAttribute in inderectAttributes)
        {
            var methods = FindAttributedMethods(inderectAttribute.AttribyteTypes);
            foreach (var x in methods)
                decorator.Decorate(x.TypeDefinition, x.MethodDefinition, inderectAttribute.HostAttribute);
        }
    }

    private HostAttributeMapping ToHostAttributeMapping(CustomAttribute arg)
    {
        var prms = arg.ConstructorArguments.First().Value as CustomAttributeArgument[];
        if (null == prms)
            return null;
        return new HostAttributeMapping
        {
            HostAttribute = arg,
            AttribyteTypes = prms.Select(c => ((TypeReference) c.Value).Resolve()).ToArray()
        };
    }

    private void DecorateDirectlyAttributed(MethodDecoratorEx.Fody.MethodDecorator decorator)
    {
        var markerTypeDefinitions = FindMarkerTypes();

        var methods = FindAttributedMethods(markerTypeDefinitions.ToArray());
        foreach (var x in methods)
            decorator.Decorate(x.TypeDefinition, x.MethodDefinition, x.CustomAttribute);
    }

    private IEnumerable<TypeDefinition> FindMarkerTypes()
    {
        var allAttributes = GetAttributes();

        var markerTypeDefinitions = (from type in allAttributes
            where HasCorrectMethods(type)
            select type).ToList();

        if (!markerTypeDefinitions.Any())
        {
            if (null != LogError)
                LogError("Could not find any method decorator attribute");
            throw new WeavingException("Could not find any method decorator attribute");
        }

        return markerTypeDefinitions;
    }

    private IEnumerable<TypeDefinition> GetAttributes()
    {
        var res = new List<TypeDefinition>();

        res.AddRange(ModuleDefinition.CustomAttributes.Select(c => c.AttributeType.Resolve()));
        res.AddRange(ModuleDefinition.Assembly.CustomAttributes.Select(c => c.AttributeType.Resolve()));

        //will find if assembly is loaded
        var methodDecorator = Type.GetType("MethodDecoratorInterfaces.IMethodDecorator, MethodDecoratorInterfaces");

        //make using of MethodDecoratorEx assembly optional because it can break exists code
        if (null != methodDecorator)
            res.AddRange(ModuleDefinition.Types.Where(c => c.Implements(methodDecorator)));

        return res;
    }

    private static bool HasCorrectMethods(TypeDefinition type)
    {
        return type.Methods.Any(IsOnEntryMethod) &&
               type.Methods.Any(IsOnExitMethod) &&
               type.Methods.Any(IsOnExceptionMethod);
    }

    private static bool IsOnEntryMethod(MethodDefinition m)
    {
        return m.Name == "OnEntry" &&
               m.Parameters.Count == 0;
    }

    private static bool IsOnExitMethod(MethodDefinition m)
    {
        return m.Name == "OnExit" &&
               m.Parameters.Count == 0;
    }

    private static bool IsOnExceptionMethod(MethodDefinition m)
    {
        return m.Name == "OnException" && m.Parameters.Count == 1 &&
               m.Parameters[0].ParameterType.FullName == typeof (Exception).FullName;
    }

    private IEnumerable<AttributeMethodInfo> FindAttributedMethods(IEnumerable<TypeDefinition> markerTypeDefintions)
    {
        return from topLevelType in ModuleDefinition.Types
            from type in GetAllTypes(topLevelType)
            from method in type.Methods
            where method.HasBody
            from attribute in method.CustomAttributes.Concat(method.DeclaringType.CustomAttributes)
            let attributeTypeDef = attribute.AttributeType.Resolve()
            from markerTypeDefinition in markerTypeDefintions
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

    private bool AreEquals(TypeDefinition attributeTypeDef, TypeDefinition markerTypeDefinition)
    {
        return attributeTypeDef.FullName == markerTypeDefinition.FullName;
    }

    private static IEnumerable<TypeDefinition> GetAllTypes(TypeDefinition type)
    {
        yield return type;

        var allNestedTypes = from t in type.NestedTypes
            from t2 in GetAllTypes(t)
            select t2;

        foreach (var t in allNestedTypes)
            yield return t;
    }

    private class HostAttributeMapping
    {
        public TypeDefinition[] AttribyteTypes { get; set; }
        public CustomAttribute HostAttribute { get; set; }
    }

    private class AttributeMethodInfo
    {
        public TypeDefinition TypeDefinition { get; set; }
        public MethodDefinition MethodDefinition { get; set; }
        public CustomAttribute CustomAttribute { get; set; }
    }
}