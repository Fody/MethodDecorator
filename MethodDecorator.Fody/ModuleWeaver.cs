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

        var markerAttributeTypeRef = FindMarkerAttribute();

        var decorator = new MethodDecorator.Fody.MethodDecorator(ModuleDefinition);

        var methods = FindAttributedMethods(markerAttributeTypeRef);
        foreach (var method in methods)
            decorator.Decorate(method.Item1, method.Item2);
    }

    private TypeReference FindMarkerAttribute()
    {
        var attributeTypeRef = (from type in ModuleDefinition.Types
                                where type.FullName == "MethodDecoratorAttribute"
                                select type).FirstOrDefault();

        if (attributeTypeRef == null)
            throw new WeavingException("Could not find type 'MethodDecoratorAttribute'");

        var onEntry = attributeTypeRef.Methods.FirstOrDefault(IsOnEntryMethod);
        var onExit = attributeTypeRef.Methods.FirstOrDefault(IsOnExitMethod);
        var onException = attributeTypeRef.Methods.FirstOrDefault(IsOnExceptionMethod);

        if (onEntry == null || onExit == null || onException == null)
            throw new WeavingException("MethodDecoratorAttribute does not contain correct OnEntry, OnExit and OnException methods");

        return attributeTypeRef;
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

    private IEnumerable<Tuple<MethodDefinition, CustomAttribute>> FindAttributedMethods(TypeReference markerAttributeTypeRef)
    {
        return from topLevelType in ModuleDefinition.Types
               from type in GetAllTypes(topLevelType)
               from method in type.Methods
               where method.HasBody
               from attribute in method.CustomAttributes
               where attribute.AttributeType.DerivesFrom(markerAttributeTypeRef)
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