using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MethodDecorator.Fody {
    public static class IlHelper {
        public static IEnumerable<Instruction> ProcessParam(ParameterDefinition parameterDefinition, VariableDefinition paramsArray) {

            var paramMetaData = parameterDefinition.ParameterType.MetadataType;
            if (paramMetaData == MetadataType.UIntPtr ||
                paramMetaData == MetadataType.FunctionPointer ||
                paramMetaData == MetadataType.IntPtr ||
                paramMetaData == MetadataType.Pointer) {
                yield break;
            }

            yield return Instruction.Create(OpCodes.Ldloc, paramsArray);
            yield return Instruction.Create(OpCodes.Ldc_I4, parameterDefinition.Index);
            yield return Instruction.Create(OpCodes.Ldarg, parameterDefinition);

            // Reset boolean flag variable to false

            // If a parameter is passed by reference then you need to use Ldind
            // ------------------------------------------------------------
            var paramType = parameterDefinition.ParameterType;

            if (paramType.IsByReference) {
                var referencedTypeSpec = (TypeSpecification)paramType;

                var pointerToValueTypeVariable = false;
                switch (referencedTypeSpec.ElementType.MetadataType) {
                        //Indirect load value of type int8 as int32 on the stack
                    case MetadataType.Boolean:
                    case MetadataType.SByte:
                        yield return Instruction.Create(OpCodes.Ldind_I1);
                        pointerToValueTypeVariable = true;
                        break;

                        // Indirect load value of type int16 as int32 on the stack
                    case MetadataType.Int16:
                        yield return Instruction.Create(OpCodes.Ldind_I2);
                        pointerToValueTypeVariable = true;
                        break;

                        // Indirect load value of type int32 as int32 on the stack
                    case MetadataType.Int32:
                        yield return Instruction.Create(OpCodes.Ldind_I4);
                        pointerToValueTypeVariable = true;
                        break;

                        // Indirect load value of type int64 as int64 on the stack
                        // Indirect load value of type unsigned int64 as int64 on the stack (alias for ldind.i8)
                    case MetadataType.Int64:
                    case MetadataType.UInt64:
                        yield return Instruction.Create(OpCodes.Ldind_I8);
                        pointerToValueTypeVariable = true;
                        break;

                        // Indirect load value of type unsigned int8 as int32 on the stack
                    case MetadataType.Byte:
                        yield return Instruction.Create(OpCodes.Ldind_U1);
                        pointerToValueTypeVariable = true;
                        break;

                        // Indirect load value of type unsigned int16 as int32 on the stack
                    case MetadataType.UInt16:
                    case MetadataType.Char:
                        yield return Instruction.Create(OpCodes.Ldind_U2);
                        pointerToValueTypeVariable = true;
                        break;

                        // Indirect load value of type unsigned int32 as int32 on the stack
                    case MetadataType.UInt32:
                        yield return Instruction.Create(OpCodes.Ldind_U4);
                        pointerToValueTypeVariable = true;
                        break;

                        // Indirect load value of type float32 as F on the stack
                    case MetadataType.Single:
                        yield return Instruction.Create(OpCodes.Ldind_R4);
                        pointerToValueTypeVariable = true;
                        break;

                        // Indirect load value of type float64 as F on the stack
                    case MetadataType.Double:
                        yield return Instruction.Create(OpCodes.Ldind_R8);
                        pointerToValueTypeVariable = true;
                        break;

                        // Indirect load value of type native int as native int on the stack
                    case MetadataType.IntPtr:
                    case MetadataType.UIntPtr:
                        yield return Instruction.Create(OpCodes.Ldind_I);
                        pointerToValueTypeVariable = true;
                        break;

                    default:
                        // Need to check if it is a value type instance, in which case
                        // we use Ldobj instruction to copy the contents of value type
                        // instance to stack and then box it
                        if (referencedTypeSpec.ElementType.IsValueType) {
                            yield return Instruction.Create(OpCodes.Ldobj, referencedTypeSpec.ElementType);
                            pointerToValueTypeVariable = true;
                        }
                        else {
                            // It is a reference type so just use reference the pointer
                            yield return Instruction.Create(OpCodes.Ldind_Ref);
                        }
                        break;
                }

                if (pointerToValueTypeVariable) {
                    // Box the de-referenced parameter type
                    yield return Instruction.Create(OpCodes.Box, referencedTypeSpec.ElementType);
                }

            }
            else {

                // If it is a value type then you need to box the instance as we are going 
                // to add it to an array which is of type object (reference type)
                // ------------------------------------------------------------
                if (paramType.IsValueType || paramType.IsGenericParameter) {
                    // Box the parameter type
                    yield return Instruction.Create(OpCodes.Box, paramType);
                }
            }

            // Store parameter in object[] array
            // ------------------------------------------------------------
            yield return Instruction.Create(OpCodes.Stelem_Ref);
        }
    }
}