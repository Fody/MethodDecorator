namespace MethodDecoratorEx.Fody
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using global::MethodDecorator.Fody;
    using Mono.Cecil;
    using Mono.Cecil.Cil;

    public class MethodDecorator
    {
        private readonly ReferenceFinder referenceFinder;

        public MethodDecorator(ModuleDefinition moduleDefinition)
        {
            this.referenceFinder = new ReferenceFinder(moduleDefinition);
        }

        public void Decorate(TypeDefinition type, MethodDefinition method, CustomAttribute attribute)
        {
            method.Body.InitLocals = true;

            var methodBaseTypeRef = this.referenceFinder.GetTypeReference(typeof(MethodBase));

            var exceptionTypeRef = this.referenceFinder.GetTypeReference(typeof(Exception));
            var parameterTypeRef = this.referenceFinder.GetTypeReference(typeof(object));
            var parametersArrayTypeRef = this.referenceFinder.GetTypeReference(typeof(object[]));

            var methodVariableDefinition = AddVariableDefinition(method, "__fody$method", methodBaseTypeRef);
            var attributeVariableDefinition = AddVariableDefinition(method, "__fody$attribute", attribute.AttributeType);
            var exceptionVariableDefinition = AddVariableDefinition(method, "__fody$exception", exceptionTypeRef);
            var parametersVariableDefinition = AddVariableDefinition(method, "__fody$parameters", parametersArrayTypeRef);

            VariableDefinition retvalVariableDefinition = null;
            if (method.ReturnType.FullName != "System.Void")
                retvalVariableDefinition = AddVariableDefinition(method, "__fody$retval", method.ReturnType);

            var initMethodRef = this.referenceFinder.GetOptionalMethodReference(attribute.AttributeType, md => md.Name == "Init");

            var onEntryMethodRef = this.referenceFinder.GetMethodReference(attribute.AttributeType, md => md.Name == "OnEntry");
            var onExitMethodRef = this.referenceFinder.GetMethodReference(attribute.AttributeType, md => md.Name == "OnExit");
            var onExceptionMethodRef = this.referenceFinder.GetMethodReference(attribute.AttributeType, md => md.Name == "OnException");
            
            var taskContinuationMethodRef = this.referenceFinder.GetOptionalMethodReference(attribute.AttributeType, md => md.Name == "TaskContinuation");

            var processor = method.Body.GetILProcessor();
            var methodBodyFirstInstruction = method.Body.Instructions.First();

            if (method.IsConstructor && method.Body.Instructions.Any(i => i.OpCode == OpCodes.Call))
            {
                methodBodyFirstInstruction = method.Body.Instructions.First(i => i.OpCode == OpCodes.Call).Next;
            }

            var initAttributeVariable = this.GetAttributeInstanceInstructions(
                processor,
                attribute,
                method,
                attributeVariableDefinition,
                methodVariableDefinition);

            IEnumerable<Instruction> callInitInstructions = null,
                createParametersArrayInstructions = null;

            if (null != initMethodRef)
            {
                createParametersArrayInstructions = CreateParametersArrayInstructions(
                    processor,
                    method,
                    parameterTypeRef,
                    parametersVariableDefinition);

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

            var taskContinuationInstructions = GetTaskContinuationInstructions(
                processor,
                retvalVariableDefinition,
                attributeVariableDefinition,
                taskContinuationMethodRef);

            ReplaceRetInstructions(processor, saveRetvalInstructions.Concat(callOnExitInstructions).First());

            processor.InsertBefore(methodBodyFirstInstruction, initAttributeVariable);

            if (null != initMethodRef)
            {
                processor.InsertBefore(methodBodyFirstInstruction, createParametersArrayInstructions);
                processor.InsertBefore(methodBodyFirstInstruction, callInitInstructions);
            }

            processor.InsertBefore(methodBodyFirstInstruction, callOnEntryInstructions);

            processor.InsertAfter(method.Body.Instructions.Last(), methodBodyReturnInstructions);
            
            processor.InsertBefore(methodBodyReturnInstruction, saveRetvalInstructions);

            processor.InsertBefore(methodBodyReturnInstruction, taskContinuationInstructions);

            processor.InsertBefore(methodBodyReturnInstruction, callOnExitInstructions);
            processor.InsertBefore(methodBodyReturnInstruction, tryCatchLeaveInstructions);

            processor.InsertBefore(methodBodyReturnInstruction, catchHandlerInstructions);


            method.Body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                CatchType = exceptionTypeRef,
                TryStart = methodBodyFirstInstruction,
                TryEnd = tryCatchLeaveInstructions.Last().Next,
                HandlerStart = catchHandlerInstructions.First(),
                HandlerEnd = catchHandlerInstructions.Last().Next
            });
        }

        private static VariableDefinition AddVariableDefinition(
            MethodDefinition method, 
            string variableName,
            TypeReference variableType)
        {
            var variableDefinition = new VariableDefinition(variableName, variableType);
            method.Body.Variables.Add(variableDefinition);
            return variableDefinition;
        }

        private static IEnumerable<Instruction> CreateParametersArrayInstructions(
            ILProcessor processor,
            MethodDefinition method, 
            TypeReference objectTypeReference /*object*/, 
            VariableDefinition arrayVariable /*parameters*/)
        {
            var createArray = new List<Instruction>
            {
                processor.Create(OpCodes.Ldc_I4, method.Parameters.Count),
                processor.Create(OpCodes.Newarr, objectTypeReference),
                processor.Create(OpCodes.Stloc, arrayVariable)
            };

            foreach (var p in method.Parameters)
                createArray.AddRange(IlHelper.ProcessParam(p, arrayVariable));

            return createArray;
        }

        private static IEnumerable<Instruction> GetTaskContinuationInstructions(
            ILProcessor processor, 
            VariableDefinition retvalVariableDefinition,
            VariableDefinition attributeVariableDefinition,
            MethodReference taskContinuationMethodReference)
        {
            if (retvalVariableDefinition == null) return new Instruction[0];
            var tr = retvalVariableDefinition.VariableType;

            if (tr.FullName.Contains("Task"))
            {
                return new[]
                {
                    processor.Create(OpCodes.Ldloc_S, attributeVariableDefinition),
                    processor.Create(OpCodes.Ldloc_S, retvalVariableDefinition),
                    processor.Create(OpCodes.Callvirt, taskContinuationMethodReference),
                };
            }

            return new Instruction[0];
        }

        private static IEnumerable<Instruction> GetCallInitInstructions(
            ILProcessor processor,
            TypeDefinition typeDefinition,
            MethodDefinition memberDefinition,
            VariableDefinition attributeVariableDefinition,
            VariableDefinition methodVariableDefinition,
            VariableDefinition parametersVariableDefinition,
            MethodReference initMethodRef)
        {
            // Call __fody$attribute.Init(this, methodBase, args)

            // start with the attribute reference
            var list = new List<Instruction>
            {
                processor.Create(OpCodes.Ldloc, attributeVariableDefinition)
            };

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

            // finally push the method base and arguments then call Init
            list.AddRange(new[]
            {
                processor.Create(OpCodes.Ldloc, methodVariableDefinition),
                processor.Create(OpCodes.Ldloc, parametersVariableDefinition),
                processor.Create(OpCodes.Callvirt, initMethodRef)
            });

            return list;
        }

        private static IEnumerable<Instruction> GetCallOnEntryInstructions(
            ILProcessor processor,
            VariableDefinition attributeVariableDefinition,
            MethodReference onEntryMethodRef)
        {
            // Call __fody$attribute.OnEntry()
            return new List<Instruction>
            {
                processor.Create(OpCodes.Ldloc, attributeVariableDefinition),
                processor.Create(OpCodes.Callvirt, onEntryMethodRef)
            };
        }

        private static IList<Instruction> GetSaveRetvalInstructions(
            ILProcessor processor,
            VariableDefinition retvalVariableDefinition)
        {
            return retvalVariableDefinition == null || processor.Body.Instructions.All(i => i.OpCode != OpCodes.Ret)
                ? new Instruction[0]
                : new[]
                {
                    processor.Create(OpCodes.Stloc_S, retvalVariableDefinition)
                };
        }

        private static IList<Instruction> GetCallOnExitInstructions(
            ILProcessor processor,
            VariableDefinition attributeVariableDefinition, 
            MethodReference onExitMethodRef)
        {
            return new List<Instruction>
            {
                processor.Create(OpCodes.Ldloc_S, attributeVariableDefinition),
                processor.Create(OpCodes.Callvirt, onExitMethodRef)
            };
        }

        private static IList<Instruction> GetMethodBodyReturnInstructions(
            ILProcessor processor,
            VariableDefinition retvalVariableDefinition)
        {
            var instructions = new List<Instruction>();
            if (retvalVariableDefinition != null)
                instructions.Add(processor.Create(OpCodes.Ldloc_S, retvalVariableDefinition));
            instructions.Add(processor.Create(OpCodes.Ret));
            return instructions;
        }

        private static IList<Instruction> GetTryCatchLeaveInstructions(
            ILProcessor processor,
            Instruction methodBodyReturnInstruction)
        {
            return new[]
            {
                processor.Create(OpCodes.Leave_S, methodBodyReturnInstruction)
            };
        }

        private static List<Instruction> GetCatchHandlerInstructions(
            ILProcessor processor,
            VariableDefinition attributeVariableDefinition, 
            VariableDefinition exceptionVariableDefinition,
            MethodReference onExceptionMethodRef)
        {
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

        private static void ReplaceRetInstructions(ILProcessor processor, Instruction methodEpilogueFirstInstruction)
        {
            // We cannot call ret inside a try/catch block. Replace all ret instructions with
            // an unconditional branch to the start of the OnExit epilogue
            var retInstructions = (from i in processor.Body.Instructions
                where i.OpCode == OpCodes.Ret
                select i).ToList();

            foreach (var instruction in retInstructions)
            {
                instruction.OpCode = OpCodes.Br_S;
                instruction.Operand = methodEpilogueFirstInstruction;
            }
        }

        private IEnumerable<Instruction> GetAttributeInstanceInstructions(
            ILProcessor processor,
            ICustomAttribute attribute,
            MethodDefinition method,
            VariableDefinition attributeVariableDefinition,
            VariableDefinition methodVariableDefinition)
        {
            var getMethodFromHandleRef = this.referenceFinder.GetMethodReference(
                typeof(MethodBase),
                md => md.Name == "GetMethodFromHandle" && md.Parameters.Count == 2);

            var getTypeof = this.referenceFinder.GetMethodReference(typeof(Type), md => md.Name == "GetTypeFromHandle");
            var ctor = this.referenceFinder.GetMethodReference(typeof(Activator), md => md.Name == "CreateInstance" && md.Parameters.Count == 1);

            return new List<Instruction>
            {
                processor.Create(OpCodes.Nop),
                processor.Create(OpCodes.Ldtoken, method),
                processor.Create(OpCodes.Ldtoken, method.DeclaringType),
                processor.Create(OpCodes.Call, getMethodFromHandleRef),

                // Push method onto the stack, GetMethodFromHandle, result on stack
                processor.Create(OpCodes.Stloc_S, methodVariableDefinition), // Store method in __fody$method
                    
                processor.Create(OpCodes.Nop),
                processor.Create(OpCodes.Ldtoken, attribute.AttributeType),
                processor.Create(OpCodes.Call, getTypeof),
                processor.Create(OpCodes.Call, ctor),
                processor.Create(OpCodes.Castclass, attribute.AttributeType),
                processor.Create(OpCodes.Stloc_S, attributeVariableDefinition)
            };
        }
    }
}