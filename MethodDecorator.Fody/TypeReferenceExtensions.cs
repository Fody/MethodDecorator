using System.Linq;
using Mono.Cecil;

public static class TypeReferenceExtensions
{
    public static bool Implements(this TypeDefinition typeDefinition, TypeReference interfaceTypeReference)
    {
        while (typeDefinition?.BaseType != null)
        {
            if (typeDefinition.Interfaces != null && typeDefinition.Interfaces.Any(i => i.InterfaceType.FullName == interfaceTypeReference.FullName))
            {
                return true;
            }

            typeDefinition = typeDefinition.BaseType.Resolve();
        }

        return false;
    }

    public static bool Implements(this TypeDefinition typeDefinition, string interfaceTypeReference)
    {
        while (typeDefinition?.BaseType != null)
        {
            if (typeDefinition.Interfaces != null && typeDefinition.Interfaces.Any(i => i.InterfaceType.FullName == interfaceTypeReference))
            {
                return true;
            }

            typeDefinition = typeDefinition.BaseType.Resolve();
        }

        return false;
    }

    public static bool DerivesFrom(this TypeReference typeReference, TypeReference expectedBaseTypeReference)
    {
        while (typeReference != null)
        {
            if (typeReference.FullName == expectedBaseTypeReference.FullName)
            {
                return true;
            }

            typeReference = typeReference.Resolve().BaseType;
        }

        return false;
    }
}