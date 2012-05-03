using System;
using System.Collections.Generic;

using Mono.Cecil;

using System.Linq;

using MethodDecorator.Fody;

public class ModuleWeaver
{
    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }

    public void Execute()
    {
        var decorator = new MethodDecorator.Fody.MethodDecorator(ModuleDefinition);

        var methods = FindAttributedMethods();
        foreach (var method in methods)
            decorator.Decorate(method.Item1, method.Item2);
    }

    private IEnumerable<Tuple<MethodDefinition, CustomAttribute>> FindAttributedMethods()
    {
        var attributedMethods = from topLevelType in ModuleDefinition.Types
                                from type in GetAllTypes(topLevelType)
                                from method in type.Methods
                                where !method.IsAbstract
                                from attribute in method.CustomAttributes
                                where attribute.AttributeType.DerivesFrom("MethodDecoratorAttribute")
                                select Tuple.Create(method, attribute);

        return attributedMethods;
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