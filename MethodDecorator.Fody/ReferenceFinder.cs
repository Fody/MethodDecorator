using System;
using System.Linq;
using Mono.Cecil;

public class ReferenceFinder
{
    ModuleDefinition moduleDefinition;

    public ReferenceFinder(ModuleDefinition moduleDefinition)
    {
        this.moduleDefinition = moduleDefinition;
    }

    public MethodReference GetMethodReference(TypeReference typeReference, Func<MethodDefinition, bool> predicate)
    {
        var typeDefinition = typeReference.Resolve();

        MethodDefinition methodDefinition;
        do
        {
            methodDefinition = typeDefinition.Methods.FirstOrDefault(predicate);
            typeDefinition = typeDefinition.BaseType?.Resolve();
        } while (methodDefinition == null && typeDefinition != null);

        return moduleDefinition.ImportReference(methodDefinition);
    }

    public MethodReference GetOptionalMethodReference(TypeReference typeReference, Func<MethodDefinition, bool> predicate)
    {
        var typeDefinition = typeReference.Resolve();

        MethodDefinition methodDefinition;
        do
        {
            methodDefinition = typeDefinition.Methods.FirstOrDefault(predicate);
            typeDefinition = typeDefinition.BaseType?.Resolve();
        } while (methodDefinition == null && typeDefinition != null);

        return null != methodDefinition ? moduleDefinition.ImportReference(methodDefinition) : null;
    }
}