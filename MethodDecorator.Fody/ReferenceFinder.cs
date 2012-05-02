using System;
using System.Linq;

using Mono.Cecil;

namespace MethodDecorator.Fody
{
    public class ReferenceFinder
    {
        private readonly ModuleDefinition moduleDefinition;

        public ReferenceFinder(ModuleDefinition moduleDefinition)
        {
            this.moduleDefinition = moduleDefinition;
        }

        public MethodReference GetMethodReference(Type declaringType, Func<MethodDefinition, bool> predicate)
        {
            return GetMethodReference(GetTypeReference(declaringType), predicate);
        }

        public MethodReference GetMethodReference(TypeReference typeReference, Func<MethodDefinition, bool> predicate)
        {
            var typeDefinition = typeReference.Resolve();

            return moduleDefinition.Import(typeDefinition.Methods.First(predicate));
        }

        public TypeReference GetTypeReference(Type type)
        {
            return moduleDefinition.Import(type);
        }
    }
}