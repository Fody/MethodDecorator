using System;
using System.Linq;

using Mono.Cecil;

namespace MethodDecorator.Fody {
    public class ReferenceFinder {
        private readonly ModuleDefinition moduleDefinition;
        private readonly ModuleDefinition mscorlib;

        public ReferenceFinder(ModuleDefinition moduleDefinition) {
            this.moduleDefinition = moduleDefinition;
            var mscorlibAssemblyReference = moduleDefinition.AssemblyReferences.First(a => a.Name == "mscorlib");
            this.mscorlib = moduleDefinition.AssemblyResolver.Resolve(mscorlibAssemblyReference).MainModule;
        }

        public MethodReference GetMethodReference(Type declaringType, Func<MethodDefinition, bool> predicate) {
            return GetMethodReference(GetTypeReference(declaringType), predicate);
        }

        public MethodReference GetMethodReference(TypeReference typeReference, Func<MethodDefinition, bool> predicate) {
            var typeDefinition = typeReference.Resolve();

            MethodDefinition methodDefinition;
            do {
                methodDefinition = typeDefinition.Methods.FirstOrDefault(predicate);
                typeDefinition = typeDefinition.BaseType == null ? null : typeDefinition.BaseType.Resolve();
            } while (methodDefinition == null && typeDefinition != null);

            return moduleDefinition.Import(methodDefinition);
        }

        public MethodReference GetOptionalMethodReference(TypeReference typeReference, Func<MethodDefinition, bool> predicate) {
            var typeDefinition = typeReference.Resolve();

            MethodDefinition methodDefinition;
            do {
                methodDefinition = typeDefinition.Methods.FirstOrDefault(predicate);
                typeDefinition = typeDefinition.BaseType == null ? null : typeDefinition.BaseType.Resolve();
            } while (methodDefinition == null && typeDefinition != null);

            return null != methodDefinition ? moduleDefinition.Import(methodDefinition) : null;
        }

        public TypeReference GetTypeReference(Type type) {

            if(type.Assembly.GetName().Name == "mscorlib") {
                var typeReference = mscorlib.Types.FirstOrDefault(tr => tr.Namespace == type.Namespace && tr.Name == type.Name);
                if(typeReference != null)
                {
                    return moduleDefinition.Import(typeReference);
                }
            }

            return moduleDefinition.Import(type);
        }
    }
}