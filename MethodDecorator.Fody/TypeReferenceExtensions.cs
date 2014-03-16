using System;
using Mono.Cecil;

namespace MethodDecorator.Fody
{
    public static class TypeReferenceExtensions
    {
        public static bool Implements<T>(this TypeDefinition typeDefinition)
        {
            return typeDefinition.Implements(typeof (T));
        }

        public static bool Implements(this TypeDefinition typeDefinition, System.Type type)
        {
            if (type.IsInterface == false)
            {
                throw new InvalidOperationException("The <type> argument ("+type.Name+") must be an Interface type.");
            }

            var referenceFinder = new ReferenceFinder(typeDefinition.Module);
            var baseTypeDefinition = referenceFinder.GetTypeReference(type);

            return typeDefinition.Implements(baseTypeDefinition);
        }

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

        public static bool DerivesFrom<T>(this TypeDefinition typeDefinition)
        {
            return typeDefinition.DerivesFrom(typeof (T));
        }

        public static bool DerivesFrom(this TypeDefinition typeDefinition, System.Type type)
        {
            var referenceFinder = new ReferenceFinder(typeDefinition.Module);
            var baseTypeDefinition = referenceFinder.GetTypeReference(type);
            return typeDefinition.DerivesFrom(baseTypeDefinition);
        }

        public static bool DerivesFrom(this TypeReference typeReference, TypeReference expectedBaseTypeReference)
        {
            while (typeReference != null)
            {
                if (typeReference.FullName == expectedBaseTypeReference.FullName)
                    return true;

                typeReference = typeReference.Resolve().BaseType;
            }

            return false;
        }
    }
}