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
        return from types in ModuleDefinition.Types
               from method in types.Methods
               from attribute in method.CustomAttributes
               where attribute.AttributeType.DerivesFrom("MethodDecoratorAttribute")
                    // TODO: Remove this testing code!!
                    && !method.Name.StartsWith("Expected")
               select Tuple.Create(method, attribute);
    }
}