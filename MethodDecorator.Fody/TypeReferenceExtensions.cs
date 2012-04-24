using Mono.Cecil;

namespace MethodDecorator.Fody
{
    public static class TypeReferenceExtensions
    {
        public static bool DerivesFrom(this TypeReference typeReference, string fullName)
        {
            while (typeReference != null)
            {
                if (typeReference.FullName == fullName)
                    return true;

                typeReference = typeReference.Resolve().BaseType;
            }

            return false;
        }
    }
}