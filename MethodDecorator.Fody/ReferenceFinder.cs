using System;
using System.Linq;

using Mono.Cecil;

public class ReferenceFinder
{
    private readonly ModuleDefinition moduleDefinition;
    private readonly ModuleDefinition mscorlib;

    public ReferenceFinder(ModuleDefinition moduleDefinition)
    {
        this.moduleDefinition = moduleDefinition;
        var mscorlibAssemblyReference = moduleDefinition.AssemblyReferences.First(a => a.Name == "mscorlib");
        mscorlib = moduleDefinition.AssemblyResolver.Resolve(mscorlibAssemblyReference).MainModule;
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