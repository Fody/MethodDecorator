using Mono.Cecil;

namespace MethodDecorator.Fody
{
    public static class TypeReferenceExtensions
    {
        public static bool Implements(this TypeDefinition typeDefinition, TypeReference interfaceTypeReference)
        {
            while(typeDefinition != null && typeDefinition.BaseType != null)
            {
                if (typeDefinition.Interfaces != null && typeDefinition.Interfaces.Contains(interfaceTypeReference))
                    return true;

                typeDefinition = typeDefinition.BaseType.Resolve();
            }

            return false;
        }

        public static bool DerivesFrom(this TypeReference typeReference, TypeReference expectedBaseTypeReference)
        {
            while (typeReference != null)
            {
                if (typeReference == expectedBaseTypeReference)
                    return true;

                typeReference = typeReference.Resolve().BaseType;
            }

            return false;
        }
    }
}