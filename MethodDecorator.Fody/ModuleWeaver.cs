using System;
using System.Collections.Generic;

using Mono.Cecil;

using System.Linq;

using MethodDecorator.Fody;

public class ModuleWeaver {
    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }

    public void Execute() {
        LogInfo = s => { };
        LogWarning = s => { };

        var markerTypeDefinitions = FindMarkerTypes();

        var decorator = new MethodDecorator.Fody.MethodDecorator(ModuleDefinition);

        var methods = FindAttributedMethods(markerTypeDefinitions);
        foreach (var method in methods)
            decorator.Decorate(method.Item1, method.Item2);
    }

    private IEnumerable<TypeDefinition> FindMarkerTypes() {
        var allAttributes = ModuleDefinition.CustomAttributes
                                            .Concat(ModuleDefinition.Assembly.CustomAttributes)
                                            .Select(x => x.AttributeType.Resolve());
                                            
        var markerTypeDefinitions = (from type in allAttributes
                                     where HasCorrectMethods(type)
                                     select type).ToList();

        if (!markerTypeDefinitions.Any())
            throw new WeavingException("Could not find any method decarator attribute");
        
        return markerTypeDefinitions;
    }

    private static bool HasCorrectMethods(TypeDefinition type) {
        return type.Methods.Any(IsOnEntryMethod) && type.Methods.Any(IsOnExitMethod) && type.Methods.Any(IsOnExceptionMethod);
    }

    private static bool IsOnEntryMethod(MethodDefinition m) {
        return m.Name == "OnEntry" &&
               m.Parameters.Count == 2 &&
               m.Parameters[0].ParameterType.FullName == typeof(System.Reflection.MethodBase).FullName &&
               m.Parameters[1].ParameterType.FullName == typeof(object[]).FullName;
    }

    private static bool IsOnExitMethod(MethodDefinition m) {
        return m.Name == "OnExit" && 
               m.Parameters.Count == 1 &&
               m.Parameters[0].ParameterType.FullName == typeof(System.Reflection.MethodBase).FullName;
    }

    private static bool IsOnExceptionMethod(MethodDefinition m) {
        return m.Name == "OnException" && m.Parameters.Count == 2 &&
               m.Parameters[0].ParameterType.FullName == typeof(System.Reflection.MethodBase).FullName &&
               m.Parameters[1].ParameterType.FullName == typeof(Exception).FullName;
    }

    private IEnumerable<Tuple<MethodDefinition, CustomAttribute>> FindAttributedMethods(IEnumerable<TypeDefinition> markerTypeDefintions) {
        return from topLevelType in ModuleDefinition.Types
               from type in GetAllTypes(topLevelType)
               from method in type.Methods
               where method.HasBody
               from attribute in method.CustomAttributes
               let attributeTypeDef = attribute.AttributeType.Resolve()
               from markerTypeDefinition in markerTypeDefintions
               where attributeTypeDef.Implements(markerTypeDefinition) || 
                     attributeTypeDef.DerivesFrom(markerTypeDefinition) ||
                     AreEquals(attributeTypeDef,markerTypeDefinition)
               select Tuple.Create(method, attribute);
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
}