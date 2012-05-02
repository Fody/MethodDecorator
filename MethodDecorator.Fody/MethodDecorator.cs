using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MethodDecorator.Fody
{
    public class MethodDecorator
    {
        private readonly ReferenceFinder referenceFinder;

        public MethodDecorator(ModuleDefinition moduleDefinition)
        {
            referenceFinder = new ReferenceFinder(moduleDefinition);
        }

        public void Decorate(MethodDefinition method, CustomAttribute attribute)
        {
            method.Body.InitLocals = true;

            var getMethodFromHandleRef = referenceFinder.GetMethodReference(typeof(MethodBase), md => md.Name == "GetMethodFromHandle" && md.Parameters.Count == 1);
            var getCustomAttributesRef = referenceFinder.GetMethodReference(typeof(MemberInfo), md => md.Name == "GetCustomAttributes" && md.Parameters.Count == 2);
            var getTypeFromHandleRef = referenceFinder.GetMethodReference(typeof(Type), md => md.Name == "GetTypeFromHandle");

            var exceptionTypeRef = referenceFinder.GetTypeReference(typeof(Exception));

            var attributeVariableDefinition = AddVariableDefinition(method, "__fody$attribute", attribute.AttributeType);
            var exceptionVariableDefinition = AddVariableDefinition(method, "__fody$exception", exceptionTypeRef);

            var onEntryMethodRef = referenceFinder.GetMethodReference(attribute.AttributeType, md => md.Name == "OnEntry");
            var onExitMethodRef = referenceFinder.GetMethodReference(attribute.AttributeType, md => md.Name == "OnExit");
            var onExceptionMethodRef = referenceFinder.GetMethodReference(attribute.AttributeType, md => md.Name == "OnException");

            var methodName = method.DeclaringType.FullName + "." + method.Name;

            var processor = method.Body.GetILProcessor();
            var methodBodyFirstInstruction = method.Body.Instructions.First();
            var methodBodyReturnInstruction = FixupMethodEndInstructions(processor);

            var getAttributeInstanceInstructions = GetAttributeInstanceInstructions(method, attribute, attributeVariableDefinition, getCustomAttributesRef, getTypeFromHandleRef, processor, getMethodFromHandleRef);
            var callOnEntryInstructions = GetCallOnEntryInstructions(methodName, onEntryMethodRef, processor, attributeVariableDefinition);
            var callOnExitInstructions = GetCallOnExitInstructions(methodName, onExitMethodRef, processor, attributeVariableDefinition);
            var tryCatchLeaveInstruction = processor.Create(OpCodes.Leave_S, methodBodyReturnInstruction);
            var catchHandlerInstructions = GetCatchHandlerInstructions(methodName, onExceptionMethodRef, attributeVariableDefinition, processor, exceptionVariableDefinition);

            processor.InsertBefore(methodBodyFirstInstruction, getAttributeInstanceInstructions);
            processor.InsertBefore(methodBodyFirstInstruction, callOnEntryInstructions);

            processor.InsertBefore(methodBodyReturnInstruction, callOnExitInstructions);
            processor.InsertBefore(methodBodyReturnInstruction, tryCatchLeaveInstruction);

            processor.InsertBefore(methodBodyReturnInstruction, catchHandlerInstructions);

            method.Body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch)
                                              {
                                                  CatchType = exceptionTypeRef,
                                                  TryStart = methodBodyFirstInstruction,
                                                  TryEnd = tryCatchLeaveInstruction.Next,
                                                  HandlerStart = catchHandlerInstructions.First(),
                                                  HandlerEnd = catchHandlerInstructions.Last().Next
                                              });
        }

        private static VariableDefinition AddVariableDefinition(MethodDefinition method, string variableName, TypeReference variableType)
        {
            var variableDefinition = new VariableDefinition(variableName, variableType);
            method.Body.Variables.Add(variableDefinition);
            return variableDefinition;
        }

        private static IEnumerable<Instruction> GetAttributeInstanceInstructions(MethodDefinition method, CustomAttribute attribute, VariableDefinition attributeVariableDefinition, MethodReference getCustomAttributesRef, MethodReference getTypeFromHandleRef, ILProcessor processor, MethodReference getMethodFromHandleRef)
        {
            // Get the attribute instance (this gets a new instance for each invocation.
            // Might be better to create a static class that keeps a track of a single
            // instance per method and we just refer to that)
            return new List<Instruction>
                   {
                       // Push method onto the stack, GetMethodFromHandle, result on stack
                       // Push attribute onto the stack, GetTypeFromHandle, result on stack
                       // Push false onto the stack, GetCustomAttributes
                       // Get 0th index
                       // Cast to attribute stor in __attribute
                       processor.Create(OpCodes.Ldtoken, method),
                       processor.Create(OpCodes.Call, getMethodFromHandleRef),
                       processor.Create(OpCodes.Ldtoken, attribute.AttributeType),
                       processor.Create(OpCodes.Call, getTypeFromHandleRef),
                       processor.Create(OpCodes.Ldc_I4_0),
                       processor.Create(OpCodes.Callvirt, getCustomAttributesRef),
                       processor.Create(OpCodes.Ldc_I4_0),
                       processor.Create(OpCodes.Ldelem_Ref),
                       processor.Create(OpCodes.Castclass, attribute.AttributeType),
                       processor.Create(OpCodes.Stloc_S, attributeVariableDefinition)
                   };
        }

        private static IEnumerable<Instruction> GetCallOnEntryInstructions(string methodName, MethodReference onEntryMethodRef, ILProcessor processor, VariableDefinition attributeVariableDefinition)
        {
            // Call __fody$attribute.OnEntry("{methodName}")
            return new List<Instruction>
                   {
                       processor.Create(OpCodes.Ldloc_S, attributeVariableDefinition),
                       processor.Create(OpCodes.Ldstr, methodName),
                       processor.Create(OpCodes.Callvirt, onEntryMethodRef)
                   };
        }

        private static IEnumerable<Instruction> GetCallOnExitInstructions(string methodName, MethodReference onExitMethodRef, ILProcessor processor, VariableDefinition attributeVariableDefinition)
        {
            // Call __fody$attribute.OnExit("{methodName}")
            return new List<Instruction>
                   {
                       processor.Create(OpCodes.Ldloc_S, attributeVariableDefinition),
                       processor.Create(OpCodes.Ldstr, methodName),
                       processor.Create(OpCodes.Callvirt, onExitMethodRef)
                   };
        }

        private static List<Instruction> GetCatchHandlerInstructions(string methodName, MethodReference onExceptionMethodRef, VariableDefinition attributeVariableDefinition, ILProcessor processor, VariableDefinition exceptionVariableDefinition)
        {
            // Store the exception in __fody$exception
            // Call __fody$attribute.OnExcetion("{methodName}", __fody$exception)
            // rethrow
            return new List<Instruction>
                   {
                       processor.Create(OpCodes.Stloc_S, exceptionVariableDefinition),
                       processor.Create(OpCodes.Ldloc_S, attributeVariableDefinition),
                       processor.Create(OpCodes.Ldstr, methodName),
                       processor.Create(OpCodes.Ldloc_S, exceptionVariableDefinition),
                       processor.Create(OpCodes.Callvirt, onExceptionMethodRef),
                       processor.Create(OpCodes.Rethrow)
                   };
        }

        private static Instruction FixupMethodEndInstructions(ILProcessor processor)
        {
            // When a method has multiple return statements, they are implemented as brances to the
            // last instruction in the method, which is a ret. We need those branches to remain inside
            // the try/catch, and branch to a point before our OnExit code, so we convert the last
            // instruction from a ret to a nop
            ConvertMethodReturnInstructionToNop(processor, processor.Body.Instructions.Last());

            // Then we need to add a new ret, which will be the target of the leave instruction
            // inside the try/catch
            var returnInstruction = processor.Create(OpCodes.Ret);
            processor.Append(returnInstruction);

            return returnInstruction;
        }

        private static void ConvertMethodReturnInstructionToNop(ILProcessor processor, Instruction lastInstruction)
        {
            lastInstruction = EnsureLastInstruction(processor, lastInstruction);

            lastInstruction.OpCode = OpCodes.Nop;
            lastInstruction.Operand = null;
        }

        private static Instruction EnsureLastInstruction(ILProcessor processor, Instruction lastInstruction)
        {
            // We're expecting a ret at the end of the method. If the method ends with a throw,
            // we don't get the ret. Add a nop so we have enough instructions to manipulate
            if (lastInstruction.OpCode == OpCodes.Throw)
            {
                lastInstruction = processor.Create(OpCodes.Nop);
                processor.Append(lastInstruction);
            }

            return lastInstruction;
        }
    }
}