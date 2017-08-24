using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace MethodDecorator.Fody {
    public class MethodDecorator {
        private readonly ReferenceFinder _referenceFinder;

        public MethodDecorator(ModuleDefinition moduleDefinition) {
            this._referenceFinder = new ReferenceFinder(moduleDefinition);
        }

        public void Decorate(TypeDefinition type, MethodDefinition method, CustomAttribute attribute, bool explicitMatch)
        {


            method.Body.InitLocals = true;
            
            var methodBaseTypeRef = this._referenceFinder.GetTypeReference(typeof(MethodBase));

            var exceptionTypeRef = this._referenceFinder.GetTypeReference(typeof(Exception));
            var parameterTypeRef = this._referenceFinder.GetTypeReference(typeof(object));
            var parametersArrayTypeRef = new ArrayType(parameterTypeRef);

            var initMethodRef1 = this._referenceFinder.GetOptionalMethodReference(attribute.AttributeType, 
                md => md.Name == "Init" && md.Parameters.Count == 1 && md.Parameters[0].ParameterType.FullName == typeof(MethodBase).FullName);
            var initMethodRef2 = this._referenceFinder.GetOptionalMethodReference(attribute.AttributeType,
                md => md.Name == "Init" && md.Parameters.Count == 2 && md.Parameters[0].ParameterType.FullName == typeof(object).FullName );
            var initMethodRef3 = this._referenceFinder.GetOptionalMethodReference(attribute.AttributeType, 
                md => md.Name == "Init" && md.Parameters.Count == 3);

            var onEntryMethodRef0 = this._referenceFinder.GetOptionalMethodReference(attribute.AttributeType, 
                md => md.Name == "OnEntry" && md.Parameters.Count == 0 );

            var onExitMethodRef0 = this._referenceFinder.GetOptionalMethodReference( attribute.AttributeType, 
                md => md.Name == "OnExit" && md.Parameters.Count == 0);
            var onExitMethodRef1 = this._referenceFinder.GetOptionalMethodReference( attribute.AttributeType, 
                md => md.Name == "OnExit" && md.Parameters.Count == 1 && md.Parameters[0].ParameterType.FullName == typeof(object).FullName);

            var onExceptionMethodRef = this._referenceFinder.GetOptionalMethodReference( attribute.AttributeType, 
                md => md.Name == "OnException" && md.Parameters.Count == 1 && md.Parameters[0].ParameterType.FullName == typeof(Exception).FullName);

            var taskContinuationMethodRef = this._referenceFinder.GetOptionalMethodReference(attribute.AttributeType, md => md.Name == "OnTaskContinuation");

            var attributeVariableDefinition = AddVariableDefinition(method, "__fody$attribute", attribute.AttributeType);
            var methodVariableDefinition = AddVariableDefinition(method, "__fody$method", methodBaseTypeRef);

            VariableDefinition exceptionVariableDefinition = null;
            VariableDefinition parametersVariableDefinition = null;
            VariableDefinition retvalVariableDefinition = null;

            if (initMethodRef3 != null)
            {
                parametersVariableDefinition = AddVariableDefinition(method, "__fody$parameters", parametersArrayTypeRef);
            }

            if (onExceptionMethodRef != null)
            {
                exceptionVariableDefinition = AddVariableDefinition(method, "__fody$exception", exceptionTypeRef);
            }

            bool needCatchReturn = null != (onExitMethodRef1 ?? onExitMethodRef0 ?? onExceptionMethodRef ?? taskContinuationMethodRef);

            if (method.ReturnType.FullName != "System.Void" && needCatchReturn)
            {
                retvalVariableDefinition = AddVariableDefinition(method, "__fody$retval", method.ReturnType);
            }

            MethodBodyRocks.SimplifyMacros(method.Body);

            var processor = method.Body.GetILProcessor();
            var methodBodyFirstInstruction = method.Body.Instructions.First();

            if (method.IsConstructor) {

                var callBase = method.Body.Instructions.FirstOrDefault(
                    i =>    (i.OpCode == OpCodes.Call) 
                            && (i.Operand is MethodReference) 
                            && ((MethodReference)i.Operand).Resolve().IsConstructor 
                            && (((MethodReference)i.Operand).DeclaringType == method.DeclaringType.BaseType 
                                || ((MethodReference)i.Operand).DeclaringType == method.DeclaringType));

                methodBodyFirstInstruction = callBase ?.Next ?? methodBodyFirstInstruction;
            }

            var initAttributeVariable = this.GetAttributeInstanceInstructions(processor,
                                                                         attribute,
                                                                         method,
                                                                         attributeVariableDefinition,
                                                                         methodVariableDefinition,
                                                                         explicitMatch);

            IEnumerable<Instruction> callInitInstructions = null,
                                     createParametersArrayInstructions = null,
                                     callOnEntryInstructions = null,
                                     saveRetvalInstructions = null,
                                     callOnExitInstructions = null;

            if (parametersVariableDefinition != null)
            {
                createParametersArrayInstructions = CreateParametersArrayInstructions(
                    processor,
                    method,
                    parameterTypeRef,
                    parametersVariableDefinition);
            }

            var initMethodRef = initMethodRef3 ?? initMethodRef2 ?? initMethodRef1;
            if(initMethodRef!=null)
            { 
                callInitInstructions = GetCallInitInstructions(
                    processor,
                    type,
                    method,
                    attributeVariableDefinition,
                    methodVariableDefinition,
                    parametersVariableDefinition,
                    initMethodRef);
            }

            if (onEntryMethodRef0 != null)
            {
                callOnEntryInstructions = GetCallOnEntryInstructions(processor, attributeVariableDefinition, onEntryMethodRef0);
            }

            if (retvalVariableDefinition != null)
            {
                saveRetvalInstructions = GetSaveRetvalInstructions(processor, retvalVariableDefinition);
            }

            if (retvalVariableDefinition!=null && onExitMethodRef1!=null)
            {
                callOnExitInstructions = GetCallOnExitInstructions(processor, attributeVariableDefinition, onExitMethodRef1, retvalVariableDefinition);
            }
            else if(onExitMethodRef0!=null)
            {
                callOnExitInstructions = GetCallOnExitInstructions(processor, attributeVariableDefinition, onExitMethodRef0);
            }

            IEnumerable<Instruction> methodBodyReturnInstructions = null,
                                     tryCatchLeaveInstructions = null,
                                     catchHandlerInstructions = null;

            if (needCatchReturn)
            {
                methodBodyReturnInstructions = GetMethodBodyReturnInstructions(processor, retvalVariableDefinition);

                if (onExceptionMethodRef != null)
                {
                    tryCatchLeaveInstructions = GetTryCatchLeaveInstructions(processor, methodBodyReturnInstructions.First());
                    catchHandlerInstructions = GetCatchHandlerInstructions(processor,
                        attributeVariableDefinition, exceptionVariableDefinition, onExceptionMethodRef);
                }

                ReplaceRetInstructions(processor, saveRetvalInstructions?.FirstOrDefault()?? callOnExitInstructions?.FirstOrDefault()?? methodBodyReturnInstructions.First());
            }

            processor.InsertBefore(methodBodyFirstInstruction, initAttributeVariable);

            if (createParametersArrayInstructions!=null)
                processor.InsertBefore(methodBodyFirstInstruction, createParametersArrayInstructions);

            if (callInitInstructions!=null) 
                processor.InsertBefore(methodBodyFirstInstruction, callInitInstructions);

            if (callOnEntryInstructions != null)
                processor.InsertBefore(methodBodyFirstInstruction, callOnEntryInstructions);

            if (methodBodyReturnInstructions != null)
            {
                processor.InsertAfter(method.Body.Instructions.Last(), methodBodyReturnInstructions);

                if(saveRetvalInstructions!=null)
                    processor.InsertBefore(methodBodyReturnInstructions.First(), saveRetvalInstructions);

                if (taskContinuationMethodRef!=null)
                {
                    var taskContinuationInstructions = GetTaskContinuationInstructions(
                        processor,
                        retvalVariableDefinition,
                        attributeVariableDefinition,
                        taskContinuationMethodRef);

                    processor.InsertBefore(methodBodyReturnInstructions.First(), taskContinuationInstructions);
                }

                if (callOnExitInstructions != null)
                    processor.InsertBefore(methodBodyReturnInstructions.First(), callOnExitInstructions);

                if (onExceptionMethodRef != null)
                {
                    processor.InsertBefore(methodBodyReturnInstructions.First(), tryCatchLeaveInstructions);
                    processor.InsertBefore(methodBodyReturnInstructions.First(), catchHandlerInstructions);

                    method.Body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch)
                    {
                        CatchType = exceptionTypeRef,
                        TryStart = methodBodyFirstInstruction,
                        TryEnd = tryCatchLeaveInstructions.Last().Next,
                        HandlerStart = catchHandlerInstructions.First(),
                        HandlerEnd = catchHandlerInstructions.Last().Next
                    });
                }
            }

            MethodBodyRocks.OptimizeMacros(method.Body);
        }

        private static VariableDefinition AddVariableDefinition(MethodDefinition method, string variableName, TypeReference variableType) {
            var variableDefinition = new VariableDefinition(variableType);
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
                createArray.AddRange(IlHelper.ProcessParam(p, arrayVariable));

            return createArray;
        }

        private IEnumerable<Instruction> GetAttributeInstanceInstructions(
            ILProcessor processor,
            ICustomAttribute attribute,
            MethodDefinition method,
            VariableDefinition attributeVariableDefinition,
            VariableDefinition methodVariableDefinition,
            bool explicitMatch) {

            var getMethodFromHandleRef = this._referenceFinder.GetMethodReference(typeof(MethodBase), md => md.Name == "GetMethodFromHandle" &&
                                                                                                            md.Parameters.Count == 2);

            var getTypeof = this._referenceFinder.GetMethodReference(typeof(Type), md => md.Name == "GetTypeFromHandle");
            var ctor = this._referenceFinder.GetMethodReference(typeof(Activator), md => md.Name == "CreateInstance" &&
                                                                                            md.Parameters.Count == 1);

            var getCustomAttrs = this._referenceFinder.GetMethodReference(typeof(Attribute), 
                md => md.Name == "GetCustomAttributes"  && 
                md.Parameters.Count == 2 && 
                md.Parameters[0].ParameterType.FullName == typeof(MemberInfo).FullName &&
                md.Parameters[1].ParameterType.FullName == typeof(Type).FullName);

            /* 
                    // Code size       23 (0x17)
                      .maxstack  1
                      .locals init ([0] class SimpleTest.IntersectMethodsMarkedByAttribute i)
                      IL_0000:  nop
                      IL_0001:  ldtoken    SimpleTest.IntersectMethodsMarkedByAttribute
                      IL_0006:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
                      IL_000b:  call       object [mscorlib]System.Activator::CreateInstance(class [mscorlib]System.Type)
                      IL_0010:  castclass  SimpleTest.IntersectMethodsMarkedByAttribute
                      IL_0015:  stloc.0
                      IL_0016:  ret
            */

            var oInstructions = new List<Instruction>
                {
                    processor.Create(OpCodes.Nop),

                    processor.Create(OpCodes.Ldtoken, method),
                    processor.Create(OpCodes.Ldtoken, method.DeclaringType),
                    processor.Create(OpCodes.Call, getMethodFromHandleRef),          // Push method onto the stack, GetMethodFromHandle, result on stack
                    processor.Create(OpCodes.Stloc, methodVariableDefinition),     // Store method in __fody$method

                    processor.Create(OpCodes.Nop),
                };

            if (explicitMatch &&
                method.CustomAttributes.Any(m => m.AttributeType.Equals(attribute.AttributeType)))
            {
                oInstructions.AddRange(new Instruction[]
                    {
                        processor.Create(OpCodes.Ldloc, methodVariableDefinition),
                        processor.Create(OpCodes.Ldtoken, attribute.AttributeType),
                        processor.Create(OpCodes.Call,getTypeof),
                        processor.Create(OpCodes.Call,getCustomAttrs),

                        processor.Create(OpCodes.Dup),
                        processor.Create(OpCodes.Ldlen),
                        processor.Create(OpCodes.Ldc_I4_1),
                        processor.Create(OpCodes.Sub),

//                      processor.Create(OpCodes.Ldc_I4_0),
                        processor.Create(OpCodes.Ldelem_Ref),

                        processor.Create(OpCodes.Castclass, attribute.AttributeType),
                        processor.Create(OpCodes.Stloc, attributeVariableDefinition),
                    });
            }
            else if (explicitMatch &&
                     method.DeclaringType.CustomAttributes.Any(m => m.AttributeType.Equals(attribute.AttributeType)))
            {
                oInstructions.AddRange(new Instruction[]
                    {
                        processor.Create(OpCodes.Ldtoken, method.DeclaringType),
                        processor.Create(OpCodes.Call,getTypeof),
                        processor.Create(OpCodes.Ldtoken, attribute.AttributeType),
                        processor.Create(OpCodes.Call,getTypeof),
                        processor.Create(OpCodes.Call,getCustomAttrs),

                        processor.Create(OpCodes.Dup),
                        processor.Create(OpCodes.Ldlen),
                        processor.Create(OpCodes.Ldc_I4_1),
                        processor.Create(OpCodes.Sub),

//                      processor.Create(OpCodes.Ldc_I4_0),
                        processor.Create(OpCodes.Ldelem_Ref),

                        processor.Create(OpCodes.Castclass, attribute.AttributeType),
                        processor.Create(OpCodes.Stloc, attributeVariableDefinition),
                    });
            }
            else
            {
                oInstructions.AddRange(new Instruction[]
                    {
                        processor.Create(OpCodes.Ldtoken, attribute.AttributeType),
                        processor.Create(OpCodes.Call,getTypeof),
                        processor.Create(OpCodes.Call,ctor),
                        processor.Create(OpCodes.Castclass, attribute.AttributeType),
                        processor.Create(OpCodes.Stloc, attributeVariableDefinition),
                    });
            }

            return oInstructions;
                    /*
                    
                     * 
                    processor.Create(OpCodes.Ldloc_S, methodVariableDefinition),
                    processor.Create(OpCodes.Ldtoken, attribute.AttributeType),
                    processor.Create(OpCodes.Call, getTypeFromHandleRef),            // Push method + attribute onto the stack, GetTypeFromHandle, result on stack
                    processor.Create(OpCodes.Ldc_I4_0),
                    processor.Create(OpCodes.Callvirt, getCustomAttributesRef),      // Push false onto the stack (result still on stack), GetCustomAttributes
                    processor.Create(OpCodes.Ldc_I4_0),
                    processor.Create(OpCodes.Ldelem_Ref),                            // Get 0th index from result
                    processor.Create(OpCodes.Castclass, attribute.AttributeType),
                    processor.Create(OpCodes.Stloc_S, attributeVariableDefinition)   // Cast to attribute stor in __fody$attribute
                    */ 
        }

        private static IEnumerable<Instruction> GetCallInitInstructions(
            ILProcessor processor,
            TypeDefinition typeDefinition,
            MethodDefinition memberDefinition,
            VariableDefinition attributeVariableDefinition,
            VariableDefinition methodVariableDefinition,
            VariableDefinition parametersVariableDefinition,
            MethodReference initMethodRef) {
            // Call __fody$attribute.Init(this, methodBase, args)

            // start with the attribute reference
            var list = new List<Instruction>
                {
                    processor.Create(OpCodes.Ldloc, attributeVariableDefinition),
                };

            if (initMethodRef.Parameters.Count > 1)
            {
                // then push the instance reference onto the stack
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
            }

            list.Add(processor.Create(OpCodes.Ldloc, methodVariableDefinition));

            if (initMethodRef.Parameters.Count > 2)
            {
                list.Add(processor.Create(OpCodes.Ldloc, parametersVariableDefinition));
            }

            list.Add(processor.Create(OpCodes.Callvirt, initMethodRef));
 
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
                new Instruction[0] : new[] { processor.Create(OpCodes.Stloc, retvalVariableDefinition) };
        }

        private static IList<Instruction> GetCallOnExitInstructions(ILProcessor processor, VariableDefinition attributeVariableDefinition, MethodReference onExitMethodRef) {
            // Call __fody$attribute.OnExit()
            return new List<Instruction>
                   {
                       processor.Create(OpCodes.Ldloc, attributeVariableDefinition),
                       //processor.Create(OpCodes.Ldarg_0),
                       processor.Create(OpCodes.Callvirt, onExitMethodRef)
                   };
        }

        private static IList<Instruction> GetCallOnExitInstructions(ILProcessor processor, 
            VariableDefinition attributeVariableDefinition, MethodReference onExitMethodRef, VariableDefinition retvalVariableDefinition)
        {
            var oInstructions = new List<Instruction>
            {
                processor.Create(OpCodes.Ldloc,  attributeVariableDefinition),
                processor.Create(OpCodes.Ldloc, retvalVariableDefinition),
            };

            if (retvalVariableDefinition.VariableType.IsValueType ||
                retvalVariableDefinition.VariableType.IsGenericParameter)
            {
                oInstructions.Add( processor.Create(OpCodes.Box, retvalVariableDefinition.VariableType));
            }

            oInstructions.Add(processor.Create(OpCodes.Callvirt, onExitMethodRef));
            return oInstructions;
        }

        private static IList<Instruction> GetMethodBodyReturnInstructions(ILProcessor processor, VariableDefinition retvalVariableDefinition) {
            var instructions = new List<Instruction>();
            if (retvalVariableDefinition != null)
                instructions.Add(processor.Create(OpCodes.Ldloc, retvalVariableDefinition));
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
                       processor.Create(OpCodes.Stloc, exceptionVariableDefinition),
                       processor.Create(OpCodes.Ldloc, attributeVariableDefinition),
                       processor.Create(OpCodes.Ldloc, exceptionVariableDefinition),
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
                instruction.OpCode = OpCodes.Br;
                instruction.Operand = methodEpilogueFirstInstruction;
            }
        }

        private static IEnumerable<Instruction> GetTaskContinuationInstructions(ILProcessor processor, VariableDefinition retvalVariableDefinition, VariableDefinition attributeVariableDefinition, MethodReference taskContinuationMethodReference) {
            if (retvalVariableDefinition != null) {
                var tr = retvalVariableDefinition.VariableType;

                if (tr.FullName.Contains("System.Threading.Tasks.Task"))
                    return new[]
                    {
                    processor.Create(OpCodes.Ldloc, attributeVariableDefinition),
                    processor.Create(OpCodes.Ldloc, retvalVariableDefinition),
                    processor.Create(OpCodes.Callvirt, taskContinuationMethodReference),
                };
            }
            return new Instruction[0];
        }
    }
}


