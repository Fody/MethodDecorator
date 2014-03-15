using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MethodDecorator.Fody {
    public class MethodDecorator {
        private readonly ReferenceFinder referenceFinder;

        public MethodDecorator(ModuleDefinition moduleDefinition) {
            referenceFinder = new ReferenceFinder(moduleDefinition);
        }

        public void Decorate(TypeDefinition type, MethodDefinition method, CustomAttribute attribute)
        {
            method.Body.InitLocals = true;

            var getMethodFromHandleRef = referenceFinder.GetMethodReference(typeof(MethodBase), md => md.Name == "GetMethodFromHandle" && md.Parameters.Count == 2);
            var getCustomAttributesRef = referenceFinder.GetMethodReference(typeof(MemberInfo), md => md.Name == "GetCustomAttributes" && md.Parameters.Count == 2);
            var getTypeFromHandleRef = referenceFinder.GetMethodReference(typeof(Type), md => md.Name == "GetTypeFromHandle");

            var methodBaseTypeRef = referenceFinder.GetTypeReference(typeof(MethodBase));
            var exceptionTypeRef = referenceFinder.GetTypeReference(typeof(Exception));
            var parameterTypeRef = referenceFinder.GetTypeReference(typeof(object));
            var parametersArrayTypeRef = referenceFinder.GetTypeReference(typeof(object[]));

            var methodVariableDefinition = AddVariableDefinition(method, "__fody$method", methodBaseTypeRef);
            var attributeVariableDefinition = AddVariableDefinition(method, "__fody$attribute", attribute.AttributeType);
            var exceptionVariableDefinition = AddVariableDefinition(method, "__fody$exception", exceptionTypeRef);
            var parametersVariableDefinition = AddVariableDefinition(method, "__fody$parameters", parametersArrayTypeRef);

            VariableDefinition retvalVariableDefinition = null;
            if (method.ReturnType.FullName != "System.Void")
                retvalVariableDefinition = AddVariableDefinition(method, "__fody$retval", method.ReturnType);

            var initMethodRef = referenceFinder.GetOptionalMethodReference(attribute.AttributeType, md => md.Name == "Init");

            var onEntryMethodRef = referenceFinder.GetMethodReference(attribute.AttributeType, md => md.Name == "OnEntry");
            var onExitMethodRef = referenceFinder.GetMethodReference(attribute.AttributeType, md => md.Name == "OnExit");
            var onExceptionMethodRef = referenceFinder.GetMethodReference(attribute.AttributeType, md => md.Name == "OnException");

            var processor = method.Body.GetILProcessor();
            var methodBodyFirstInstruction = method.Body.Instructions.First();
            if (method.IsConstructor)
                methodBodyFirstInstruction = method.Body.Instructions.First(i => i.OpCode == OpCodes.Call).Next;

            var getAttributeInstanceInstructions = GetAttributeInstanceInstructions(processor, method, attribute, attributeVariableDefinition, methodVariableDefinition, getCustomAttributesRef, getTypeFromHandleRef, getMethodFromHandleRef);

            var createParametersArrayInstructions = CreateParametersArrayInstructions(processor, method, parameterTypeRef, parametersVariableDefinition);

            IEnumerable<Instruction> callInitInstructions = null;
            
            if (null != initMethodRef) {
                callInitInstructions = GetCallInitInstructions(
                    processor,
                    type,
                    method,
                    attributeVariableDefinition,
                    methodVariableDefinition,
                    parametersVariableDefinition,
                    initMethodRef);
            }

            var callOnEntryInstructions = GetCallOnEntryInstructions(processor, attributeVariableDefinition, onEntryMethodRef);
            var saveRetvalInstructions = GetSaveRetvalInstructions(processor, retvalVariableDefinition);
            var callOnExitInstructions = GetCallOnExitInstructions(processor, attributeVariableDefinition, onExitMethodRef);
            var methodBodyReturnInstructions = GetMethodBodyReturnInstructions(processor, retvalVariableDefinition);
            var methodBodyReturnInstruction = methodBodyReturnInstructions.First();
            var tryCatchLeaveInstructions = GetTryCatchLeaveInstructions(processor, methodBodyReturnInstruction);
            var catchHandlerInstructions = GetCatchHandlerInstructions(processor, attributeVariableDefinition, exceptionVariableDefinition, onExceptionMethodRef);

            ReplaceRetInstructions(processor, saveRetvalInstructions.Concat(callOnExitInstructions).First());

            processor.InsertBefore(methodBodyFirstInstruction, getAttributeInstanceInstructions);
            processor.InsertBefore(methodBodyFirstInstruction, createParametersArrayInstructions);

            if (null != initMethodRef)
                processor.InsertBefore(methodBodyFirstInstruction, callInitInstructions);

            processor.InsertBefore(methodBodyFirstInstruction, callOnEntryInstructions);

            processor.InsertAfter(method.Body.Instructions.Last(), methodBodyReturnInstructions);

            processor.InsertBefore(methodBodyReturnInstruction, saveRetvalInstructions);
            processor.InsertBefore(methodBodyReturnInstruction, callOnExitInstructions);
            processor.InsertBefore(methodBodyReturnInstruction, tryCatchLeaveInstructions);

            processor.InsertBefore(methodBodyReturnInstruction, catchHandlerInstructions);

            method.Body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch) {
                CatchType = exceptionTypeRef,
                TryStart = methodBodyFirstInstruction,
                TryEnd = tryCatchLeaveInstructions.Last().Next,
                HandlerStart = catchHandlerInstructions.First(),
                HandlerEnd = catchHandlerInstructions.Last().Next
            });
        }

        private static VariableDefinition AddVariableDefinition(MethodDefinition method, string variableName, TypeReference variableType) {
            var variableDefinition = new VariableDefinition(variableName, variableType);
            method.Body.Variables.Add(variableDefinition);
            return variableDefinition;
        }

        private static IEnumerable<Instruction> CreateParametersArrayInstructions(ILProcessor processor, MethodDefinition method, TypeReference objectTypeReference /*object*/, VariableDefinition arrayVariable /*parameters*/) {
            var createArray = new List<Instruction> {
                processor.Create(OpCodes.Ldc_I4, method.Parameters.Count),  //method.Parameters.Count
                processor.Create(OpCodes.Newarr, objectTypeReference),      // new object[method.Parameters.Count]
                processor.Create(OpCodes.Stloc, arrayVariable)              // var objArray = new object[method.Parameters.Count]
            };

            foreach (var p in method.Parameters)
                createArray.AddRange(ProcessParam(p, arrayVariable));

            return createArray;
        }

        private static IEnumerable<Instruction> GetAttributeInstanceInstructions(
            ILProcessor processor,
            MethodReference method,
            ICustomAttribute attribute,
            VariableDefinition attributeVariableDefinition,
            VariableDefinition methodVariableDefinition,
            MethodReference getCustomAttributesRef,
            MethodReference getTypeFromHandleRef,
            MethodReference getMethodFromHandleRef) {
            // Get the attribute instance (this gets a new instance for each invocation.
            // Might be better to create a static class that keeps a track of a single
            // instance per method and we just refer to that)
            return new List<Instruction>
                   {
                       processor.Create(OpCodes.Ldtoken, method),
                       processor.Create(OpCodes.Ldtoken, method.DeclaringType),
                       processor.Create(OpCodes.Call, getMethodFromHandleRef),          // Push method onto the stack, GetMethodFromHandle, result on stack
                       processor.Create(OpCodes.Stloc_S, methodVariableDefinition),     // Store method in __fody$method
                       processor.Create(OpCodes.Ldloc_S, methodVariableDefinition),
                       processor.Create(OpCodes.Ldtoken, attribute.AttributeType),
                       processor.Create(OpCodes.Call, getTypeFromHandleRef),            // Push method + attribute onto the stack, GetTypeFromHandle, result on stack
                       processor.Create(OpCodes.Ldc_I4_0),
                       processor.Create(OpCodes.Callvirt, getCustomAttributesRef),      // Push false onto the stack (result still on stack), GetCustomAttributes
                       processor.Create(OpCodes.Ldc_I4_0),
                       processor.Create(OpCodes.Ldelem_Ref),                            // Get 0th index from result
                       processor.Create(OpCodes.Castclass, attribute.AttributeType),
                       processor.Create(OpCodes.Stloc_S, attributeVariableDefinition)   // Cast to attribute stor in __fody$attribute
                   };
        }

        private static IEnumerable<Instruction> GetCallInitInstructions(ILProcessor processor, TypeDefinition typeDefinition, MethodDefinition memberDefinition, VariableDefinition attributeVariableDefinition, VariableDefinition methodVariableDefinition, VariableDefinition parametersVariableDefinition, MethodReference initMethodRef) {
            // Call __fody$attribute.Init(this, methodBase, args)
            var list = new List<Instruction>
                {
                    processor.Create(OpCodes.Ldloc, attributeVariableDefinition),
                };

            if (memberDefinition.IsConstructor || memberDefinition.IsStatic)
            {
                list.Add(processor.Create(OpCodes.Ldnull));
            }
            else
            {
                list.Add(processor.Create(OpCodes.Ldarg_0));
                if (typeDefinition.IsValueType)
                {
                    list.Add(processor.Create(OpCodes.Box, typeDefinition));
                }
            }
            //                                ? /* load 'null' */ OpCodes.Ldnull // (b/c 'this' is not available)
             //                               : /* load 'this' */ OpCodes.Ldarg_0), 
            list.AddRange(new[]
                {
                    processor.Create(OpCodes.Ldloc, methodVariableDefinition),
                    processor.Create(OpCodes.Ldloc, parametersVariableDefinition),
                    processor.Create(OpCodes.Callvirt, initMethodRef),
                });

            return list;
        }

        private static IEnumerable<Instruction> GetCallOnEntryInstructions(
            ILProcessor processor,
            VariableDefinition attributeVariableDefinition,
            MethodReference onEntryMethodRef) {
            // Call __fody$attribute.OnEntry()
            return new List<Instruction>
                   {
                       processor.Create(OpCodes.Ldloc, attributeVariableDefinition),
                       processor.Create(OpCodes.Callvirt, onEntryMethodRef),
                   };
        }

        private static IList<Instruction> GetSaveRetvalInstructions(ILProcessor processor, VariableDefinition retvalVariableDefinition) {
            return retvalVariableDefinition == null || processor.Body.Instructions.All(i => i.OpCode != OpCodes.Ret) ?
                new Instruction[0] : new[] { processor.Create(OpCodes.Stloc_S, retvalVariableDefinition) };
        }

        private static IList<Instruction> GetCallOnExitInstructions(ILProcessor processor, VariableDefinition attributeVariableDefinition, MethodReference onExitMethodRef) {
            // Call __fody$attribute.OnExit()
            return new List<Instruction>
                   {
                       processor.Create(OpCodes.Ldloc_S, attributeVariableDefinition),
                       //processor.Create(OpCodes.Ldarg_0),
                       processor.Create(OpCodes.Callvirt, onExitMethodRef)
                   };
        }

        private static IList<Instruction> GetMethodBodyReturnInstructions(ILProcessor processor, VariableDefinition retvalVariableDefinition) {
            var instructions = new List<Instruction>();
            if (retvalVariableDefinition != null)
                instructions.Add(processor.Create(OpCodes.Ldloc_S, retvalVariableDefinition));
            instructions.Add(processor.Create(OpCodes.Ret));
            return instructions;
        }

        private static IList<Instruction> GetTryCatchLeaveInstructions(ILProcessor processor, Instruction methodBodyReturnInstruction) {
            return new[] { processor.Create(OpCodes.Leave_S, methodBodyReturnInstruction) };
        }

        private static List<Instruction> GetCatchHandlerInstructions(ILProcessor processor, VariableDefinition attributeVariableDefinition, VariableDefinition exceptionVariableDefinition, MethodReference onExceptionMethodRef) {
            // Store the exception in __fody$exception
            // Call __fody$attribute.OnExcetion("{methodName}", __fody$exception)
            // rethrow
            return new List<Instruction>
                   {
                       processor.Create(OpCodes.Stloc_S, exceptionVariableDefinition),
                       processor.Create(OpCodes.Ldloc_S, attributeVariableDefinition),
                       processor.Create(OpCodes.Ldloc_S, exceptionVariableDefinition),
                       processor.Create(OpCodes.Callvirt, onExceptionMethodRef),
                       processor.Create(OpCodes.Rethrow)
                   };
        }

        private static void ReplaceRetInstructions(ILProcessor processor, Instruction methodEpilogueFirstInstruction) {
            // We cannot call ret inside a try/catch block. Replace all ret instructions with
            // an unconditional branch to the start of the OnExit epilogue
            var retInstructions = (from i in processor.Body.Instructions
                                   where i.OpCode == OpCodes.Ret
                                   select i).ToList();

            foreach (var instruction in retInstructions) {
                instruction.OpCode = OpCodes.Br_S;
                instruction.Operand = methodEpilogueFirstInstruction;
            }
        }

        private static IEnumerable<Instruction> ProcessParam(ParameterDefinition parameterDefinition, VariableDefinition paramsArray) {

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


