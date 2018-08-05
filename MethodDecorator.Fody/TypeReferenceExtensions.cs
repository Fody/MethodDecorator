using System;
using System.Linq;
using MethodDecorator.Fody;
using Mono.Cecil;

public static class TypeReferenceExtensions
{
    public static bool Implements(this TypeDefinition typeDefinition, Type type)
    {
        if (type.IsInterface == false)
        {
            throw new InvalidOperationException($"The <type> argument ({type.Name}) must be an Interface type.");
        }

        var referenceFinder = new ReferenceFinder(typeDefinition.Module);
        var baseTypeDefinition = referenceFinder.GetTypeReference(type);

        return typeDefinition.Implements(baseTypeDefinition);
    }

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