using System;
using System.Collections.Generic;
using System.Linq;

using MethodDecorator.AOP;

using MethodDecoratorEx.Fody;

using Mono.Cecil;

public class ModuleWeaver {
    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }
    public Action<string> LogError { get; set; }
    

    public void Execute() {
        this.LogInfo = s => { };
        this.LogWarning = s => { };

        var markerTypeDefinitions = this.FindMarkerTypes();

        var decorator = new MethodDecoratorEx.Fody.MethodDecorator(this.ModuleDefinition);

        var methods = this.FindAttributedMethods(markerTypeDefinitions.ToArray());
        foreach (var x in methods)
            decorator.Decorate(x.TypeDefinition, x.MethodDefinition, x.CustomAttribute);
    }

    private IEnumerable<TypeDefinition> FindMarkerTypes() {
        var allAttributes = this.ModuleDefinition.Types.Where(t => t.Implements<IMethodDecorator>())
                                .Concat(this.ModuleDefinition.CustomAttributes.Select(c => c.AttributeType.Resolve()))
                                .Concat(this.ModuleDefinition.Assembly.CustomAttributes.Select(c => c.AttributeType.Resolve()));

        var arguments =
            this.ModuleDefinition.CustomAttributes.Where(x => x.HasConstructorArguments)
                .SelectMany(x => x.ConstructorArguments);


        var markerTypeDefinitions = (from type in allAttributes
                                     where HasCorrectMethods(type)
                                     select type).ToList();

        if (!markerTypeDefinitions.Any()) {
            if (null != LogError)
                LogError("Could not find any method decorator attribute");
            throw new WeavingException("Could not find any method decorator attribute");
        }

        return markerTypeDefinitions;
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

    private IEnumerable<AttributeMethodInfo> FindAttributedMethods(ICollection<TypeDefinition> markerTypeDefintions) {
        return from topLevelType in this.ModuleDefinition.Types
            from type in GetAllTypes(topLevelType)
            from method in type.Methods
            where method.HasBody
            from attribute in method.CustomAttributes
            let attributeTypeDef = attribute.AttributeType.Resolve()
            from markerTypeDefinition in markerTypeDefintions
            where attributeTypeDef.Implements(markerTypeDefinition) ||
                  attributeTypeDef.DerivesFrom(markerTypeDefinition) ||
                  this.AreEquals(attributeTypeDef, markerTypeDefinition)
            select new AttributeMethodInfo {
                           CustomAttribute = attribute,
                           TypeDefinition = type,
                           MethodDefinition = method
                       };
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

    private class AttributeMethodInfo {
        public TypeDefinition TypeDefinition { get; set; }
        public MethodDefinition MethodDefinition { get; set; }
        public CustomAttribute CustomAttribute { get; set; }
    }
}