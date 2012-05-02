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
        private readonly ModuleDefinition moduleDefinition;

        public MethodDecorator(ModuleDefinition moduleDefinition)
        {
            this.moduleDefinition = moduleDefinition;
        }

        public void Decorate(MethodDefinition method, CustomAttribute attribute)
        {
            method.Body.InitLocals = true;

            var attributeVariableDefinition = new VariableDefinition("__fody$attribute", attribute.AttributeType);
            method.Body.Variables.Add(attributeVariableDefinition);

            var methodBaseTypeRef = moduleDefinition.Import(typeof(MethodBase));
            var methodBaseTypeDef = methodBaseTypeRef.Resolve();
            var getMethodFromHandleRef = moduleDefinition.Import(methodBaseTypeDef.Methods.First(md => md.Name == "GetMethodFromHandle" && md.Parameters.Count == 1));
            var memberInfoTypeRef = moduleDefinition.Import(typeof(MemberInfo));
            var memberInfoTypeDef = memberInfoTypeRef.Resolve();
            var getCustomAttributesRef = moduleDefinition.Import(memberInfoTypeDef.Methods.First(md => md.Name == "GetCustomAttributes" && md.Parameters.Count == 2));

            var typeTypeRef = moduleDefinition.Import(typeof(Type));
            var typeTypeDef = typeTypeRef.Resolve();
            var getTypeFromHandleRef = moduleDefinition.Import(typeTypeDef.Methods.First(md => md.Name == "GetTypeFromHandle"));

            var exceptionTypeRef = moduleDefinition.Import(typeof(Exception));

            var exceptionVariableDefinition = new VariableDefinition("__fody$exception", exceptionTypeRef);
            method.Body.Variables.Add(exceptionVariableDefinition);


            var processor = method.Body.GetILProcessor();

            var originalFirstInstruction = method.Body.Instructions[0];
            var originalLastInstruction = method.Body.Instructions[method.Body.Instructions.Count - 1];

            if (originalLastInstruction.OpCode != OpCodes.Ret)
            {
                // TODO: Will this cause a stack underflow for a non-void method?
                originalLastInstruction = processor.Create(OpCodes.Ret);
                processor.Append(originalLastInstruction);
            }

            // Get the attribute instance (this gets a new instance for each invocation.
            // Might be better to create a static class that keeps a track of a single
            // instance per method and we just refer to that)
            var getAttributeInstanceInstructions = new List<Instruction>
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

            var onEntryMethodRef = attribute.AttributeType.Resolve().Methods.First(md => md.Name == "OnEntry");
            var onExitMethodRef = attribute.AttributeType.Resolve().Methods.First(md => md.Name == "OnExit");
            var onExceptionMethodRef = attribute.AttributeType.Resolve().Methods.First(md => md.Name == "OnException");

            var methodName = method.DeclaringType.FullName + "." + method.Name;

            // Call __fody$attribute.OnEntry("{methodName}")
            var callOnEntryInstructions = new List<Instruction>
                                          {
                                              processor.Create(OpCodes.Ldloc_S, attributeVariableDefinition),
                                              processor.Create(OpCodes.Ldstr, methodName),
                                              processor.Create(OpCodes.Callvirt, onEntryMethodRef)
                                          };
            
            // Call __fody$attribute.OnExit("{methodName}")
            var callOnExitInstructions = new List<Instruction>
                                         {
                                             processor.Create(OpCodes.Ldloc_S, attributeVariableDefinition),
                                             processor.Create(OpCodes.Ldstr, methodName),
                                             processor.Create(OpCodes.Callvirt, onExitMethodRef)
                                         };

            var tryEpilogueInstructions = new List<Instruction>
                                          {
                                              processor.Create(OpCodes.Leave_S, originalLastInstruction)
                                          };

            var catchHandlerInstructions = new List<Instruction>
                                           {
                                               processor.Create(OpCodes.Stloc_S, exceptionVariableDefinition),
                                               processor.Create(OpCodes.Ldloc_S, attributeVariableDefinition),
                                               processor.Create(OpCodes.Ldstr, methodName),
                                               processor.Create(OpCodes.Ldloc_S, exceptionVariableDefinition),
                                               processor.Create(OpCodes.Callvirt, onExceptionMethodRef),
                                               processor.Create(OpCodes.Rethrow)
                                           };


            foreach (var instruction in getAttributeInstanceInstructions)
                processor.InsertBefore(originalFirstInstruction, instruction);
            foreach (var instruction in callOnEntryInstructions)
                processor.InsertBefore(originalFirstInstruction, instruction);
            foreach (var instruction in callOnExitInstructions)
                processor.InsertBefore(originalLastInstruction, instruction);
            foreach (var instruction in tryEpilogueInstructions)
                processor.InsertBefore(originalLastInstruction, instruction);
            foreach (var instruction in catchHandlerInstructions)
                processor.InsertBefore(originalLastInstruction, instruction);

            method.Body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch)
                                              {
                                                  CatchType = exceptionTypeRef,
                                                  TryStart = originalFirstInstruction,
                                                  TryEnd = tryEpilogueInstructions.Last().Next,
                                                  HandlerStart = catchHandlerInstructions.First(),
                                                  HandlerEnd = catchHandlerInstructions.Last().Next
                                              });

            //// Get the return instruction at the end of the end of the method
            //var retInstruction = method.Body.Instructions.Last();
            //var tryEnd = retInstruction;

            //if (retInstruction.OpCode == OpCodes.Ret)
            //{
            //    tryEnd = retInstruction.Previous;
            //    processor.Remove(retInstruction);
            //    processor.Append(OpCodes.Leave_S, );
            //}

            //var catchStart = processor.Create(OpCodes.Nop);
            //processor.Append(catchStart);

            //var catchEnd = processor.Create(OpCodes.Rethrow);
            //processor.Append(catchEnd);

            //method.Body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch)
            //                                  {
            //                                      CatchType = exceptionTypeDef,
            //                                      TryStart = originalFirstInstruction,
            //                                      TryEnd = tryEnd.Next,
            //                                      HandlerStart = catchStart,
            //                                      HandlerEnd = catchEnd.Next
            //                                  });

            //// Call __fody$attribute.OnExit("{methodName}")
            //var callOnExitInstructions = new List<Instruction>
            //                             {
            //                                 processor.Create(OpCodes.Ldloc_S, attributeVariableDefinition),
            //                                 processor.Create(OpCodes.Ldstr, methodName),
            //                                 processor.Create(OpCodes.Callvirt, onEntryMethodRef)
            //                             };
        }
    }
}