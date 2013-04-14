using System;
using System.Collections.Generic;

using Mono.Cecil;

using System.Linq;

using MethodDecorator.Fody;

public class ModuleWeaver
{
    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }

    public void Execute()
    {
        LogInfo = s => { };
        LogWarning = s => { };

        var markerTypeDefinitions = FindMarkerTypes();

        var decorator = new MethodDecorator.Fody.MethodDecorator(ModuleDefinition);

        var methods = FindAttributedMethods(markerTypeDefinitions);
        foreach (var method in methods)
            decorator.Decorate(method.Item1, method.Item2);
    }

    private IList<TypeDefinition> FindMarkerTypes()
    {
        var markerTypeDefinitions = (from type in ModuleDefinition.Types
                                     where type.FullName == "IMethodDecorator" || type.FullName == "MethodDecoratorAttribute"
                                     select type).ToList();

        if (!markerTypeDefinitions.Any())
        {
            var markerTypeDefinitionsAll = (from type in ModuleDefinition.Types
                                            where type.Name == "IMethodDecorator" || type.Name == "MethodDecoratorAttribute"
                                            select type).ToList();
            if(markerTypeDefinitionsAll.Any())
                throw new WeavingException("Could not find type 'IMethodDecorator' or 'MethodDecoratorAttribute' (must be in the global namespace)");
            throw new WeavingException("Could not find type 'IMethodDecorator' or 'MethodDecoratorAttribute'");
        }

        if (!markerTypeDefinitions.Any(HasCorrectMethods))
            throw new WeavingException("IMethodDecorator does not contain correct OnEntry, OnExit and OnException methods");

        return markerTypeDefinitions;
    }

    private static bool HasCorrectMethods(TypeDefinition type)
    {
        return type.Methods.Any(IsOnEntryMethod) && type.Methods.Any(IsOnExitMethod) && type.Methods.Any(IsOnExceptionMethod);
    }

    private static bool IsOnEntryMethod(MethodDefinition m)
    {
        return m.Name == "OnEntry" && m.Parameters.Count == 1 && m.Parameters[0].ParameterType.FullName == "System.Reflection.MethodBase";
    }

    private static bool IsOnExitMethod(MethodDefinition m)
    {
        return m.Name == "OnExit" && m.Parameters.Count == 1 && m.Parameters[0].ParameterType.FullName == "System.Reflection.MethodBase";
    }

    private static bool IsOnExceptionMethod(MethodDefinition m)
    {
        return m.Name == "OnException" && m.Parameters.Count == 2
            && m.Parameters[0].ParameterType.FullName == "System.Reflection.MethodBase"
            && m.Parameters[1].ParameterType.FullName == "System.Exception";
    }

    private IEnumerable<Tuple<MethodDefinition, CustomAttribute>> FindAttributedMethods(IEnumerable<TypeDefinition> markerTypeDefintions)
    {
        return from topLevelType in ModuleDefinition.Types
               from type in GetAllTypes(topLevelType)
               from method in type.Methods
               where method.HasBody
               from attribute in method.CustomAttributes
               let attributeTypeDef = attribute.AttributeType.Resolve()
               from markerTypeDefinition in markerTypeDefintions
               where attributeTypeDef.Implements(markerTypeDefinition) || attributeTypeDef.DerivesFrom(markerTypeDefinition)
               select Tuple.Create(method, attribute);
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
}