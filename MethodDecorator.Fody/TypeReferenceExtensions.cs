using Mono.Cecil;

namespace MethodDecorator.Fody
{
    public static class TypeReferenceExtensions
    {
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