using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public partial class ModuleWeaver
{
    ReferenceFinder referenceFinder;

    public void Decorate(TypeDefinition type, MethodDefinition method, CustomAttribute attribute, bool explicitMatch)
    {
        method.Body.InitLocals = true;

        var parametersArrayTypeRef = new ArrayType(objectTypeRef);

        var initMethodRef1 = referenceFinder.GetOptionalMethodReference(attribute.AttributeType,
            md => md.Name == "Init" && md.Parameters.Count == 1 && md.Parameters[0].ParameterType.FullName == typeof(MethodBase).FullName);
        var initMethodRef2 = referenceFinder.GetOptionalMethodReference(attribute.AttributeType,
            md => md.Name == "Init" && md.Parameters.Count == 2 && md.Parameters[0].ParameterType.FullName == typeof(object).FullName);
        var initMethodRef3 = referenceFinder.GetOptionalMethodReference(attribute.AttributeType,
            md => md.Name == "Init" && md.Parameters.Count == 3);

        var needBypassRef0 = referenceFinder.GetOptionalMethodReference(attribute.AttributeType,
            md => md.Name == "NeedBypass" && md.Parameters.Count == 0);

        var onEntryMethodRef0 = referenceFinder.GetOptionalMethodReference(attribute.AttributeType,
            md => md.Name == "OnEntry" && md.Parameters.Count == 0);

        var onExitMethodRef0 = referenceFinder.GetOptionalMethodReference(attribute.AttributeType,
            md => md.Name == "OnExit" && md.Parameters.Count == 0);
        var onExitMethodRef1 = referenceFinder.GetOptionalMethodReference(attribute.AttributeType,
            md => md.Name == "OnExit" && md.Parameters.Count == 1 && md.Parameters[0].ParameterType.FullName == typeof(object).FullName);

        var alterRetvalRef1 = referenceFinder.GetOptionalMethodReference(attribute.AttributeType,
            md => md.Name == "AlterRetval" && md.Parameters.Count == 1);

        var onExceptionMethodRef = referenceFinder.GetOptionalMethodReference(attribute.AttributeType,
            md => md.Name == "OnException" && md.Parameters.Count == 1 && md.Parameters[0].ParameterType.FullName == typeof(Exception).FullName);

        var taskContinuationMethodRef = referenceFinder.GetOptionalMethodReference(attribute.AttributeType, md => md.Name == "OnTaskContinuation");

        var attributeVariableDefinition = AddVariableDefinition(method, attribute.AttributeType);
        var methodVariableDefinition = AddVariableDefinition(method, methodBaseTypeRef);

        VariableDefinition exceptionVariableDefinition = null;
        VariableDefinition parametersVariableDefinition = null;
        VariableDefinition retvalVariableDefinition = null;

        if (initMethodRef3 != null)
        {
            parametersVariableDefinition = AddVariableDefinition(method, parametersArrayTypeRef);
        }

        if (onExceptionMethodRef != null)
        {
            exceptionVariableDefinition = AddVariableDefinition(method, exceptionTypeRef);
        }

        var needCatchReturn = null != (onExitMethodRef1 ?? onExitMethodRef0 ?? onExceptionMethodRef ?? taskContinuationMethodRef ?? alterRetvalRef1 ?? needBypassRef0);

        if (method.ReturnType.FullName != "System.Void" && needCatchReturn)
        {
            retvalVariableDefinition = AddVariableDefinition(method, method.ReturnType);
        }

        using var context = new MethodEditContext(method);

        var processor = method.Body.GetILProcessor();
        var methodBodyFirstInstruction = method.Body.Instructions.First();

        if (method.IsConstructor)
        {
            var callBase = method.Body.Instructions.FirstOrDefault(
                i => i.OpCode == OpCodes.Call
                     && (i.Operand is MethodReference reference)
                     && reference.Resolve().IsConstructor);

            methodBodyFirstInstruction = callBase?.Next ?? methodBodyFirstInstruction;
        }

        var initAttributeVariable = GetAttributeInstanceInstructions(processor,
            attribute,
            method,
            attributeVariableDefinition,
            methodVariableDefinition,
            explicitMatch);

        List<Instruction> callInitInstructions = null;
        List<Instruction> createParametersArrayInstructions = null;
        List<Instruction> callOnEntryInstructions = null;
        List<Instruction> saveRetvalInstructions = null;
        List<Instruction> callOnExitInstructions = null;

        if (parametersVariableDefinition != null)
        {
            createParametersArrayInstructions = CreateParametersArrayInstructions(
                processor,
                method,
                parametersVariableDefinition).ToList();
        }

        var initMethodRef = initMethodRef3 ?? initMethodRef2 ?? initMethodRef1;
        if (initMethodRef != null)
        {
            callInitInstructions = GetCallInitInstructions(
                processor,
                type,
                method,
                attributeVariableDefinition,
                methodVariableDefinition,
                parametersVariableDefinition,
                initMethodRef).ToList();
        }

        if (onEntryMethodRef0 != null)
        {
            callOnEntryInstructions = GetCallOnEntryInstructions(processor, attributeVariableDefinition, onEntryMethodRef0).ToList();
        }

        if (retvalVariableDefinition != null)
        {
            saveRetvalInstructions = GetSaveRetvalInstructions(processor, retvalVariableDefinition).ToList();
        }

        if (retvalVariableDefinition != null && onExitMethodRef1 != null)
        {
            callOnExitInstructions = GetCallOnExitInstructions(processor, attributeVariableDefinition, onExitMethodRef1, retvalVariableDefinition).ToList();
        }
        else if (onExitMethodRef0 != null)
        {
            callOnExitInstructions = GetCallOnExitInstructions(processor, attributeVariableDefinition, onExitMethodRef0).ToList();
        }

        List<Instruction> methodBodyReturnInstructions = null;
        List<Instruction> tryCatchLeaveInstructions = null;
        List<Instruction> catchHandlerInstructions = null;
        List<Instruction> bypassInstructions = null;

        if (needCatchReturn)
        {
            methodBodyReturnInstructions = GetMethodBodyReturnInstructions(processor, attributeVariableDefinition, retvalVariableDefinition, alterRetvalRef1).ToList();

            if (needBypassRef0 != null)
            {
                bypassInstructions = GetBypassInstructions(processor, attributeVariableDefinition, needBypassRef0, methodBodyReturnInstructions.First()).ToList();
            }

            if (onExceptionMethodRef != null)
            {
                tryCatchLeaveInstructions = GetTryCatchLeaveInstructions(processor, methodBodyReturnInstructions.First()).ToList();
                catchHandlerInstructions = GetCatchHandlerInstructions(processor,
                    attributeVariableDefinition, exceptionVariableDefinition, onExceptionMethodRef);
            }

            ReplaceRetInstructions(processor, saveRetvalInstructions?.FirstOrDefault() ?? callOnExitInstructions?.FirstOrDefault() ?? methodBodyReturnInstructions.First());
        }

        processor.InsertBefore(methodBodyFirstInstruction, initAttributeVariable);

        if (createParametersArrayInstructions != null)
        {
            processor.InsertBefore(methodBodyFirstInstruction, createParametersArrayInstructions);
        }

        if (callInitInstructions != null)
        {
            processor.InsertBefore(methodBodyFirstInstruction, callInitInstructions);
        }

        if (bypassInstructions != null)
        {
            processor.InsertBefore(methodBodyFirstInstruction, bypassInstructions);
        }

        if (callOnEntryInstructions != null)
        {
            processor.InsertBefore(methodBodyFirstInstruction, callOnEntryInstructions);
        }

        if (methodBodyReturnInstructions != null)
        {
            processor.InsertAfter(method.Body.Instructions.Last(), methodBodyReturnInstructions);

            if (saveRetvalInstructions != null)
                processor.InsertBefore(methodBodyReturnInstructions.First(), saveRetvalInstructions);

            if (taskContinuationMethodRef != null)
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
    }

    static VariableDefinition AddVariableDefinition(MethodDefinition method, TypeReference variableType)
    {
        var variableDefinition = new VariableDefinition(variableType);
        method.Body.Variables.Add(variableDefinition);
        return variableDefinition;
    }

    IEnumerable<Instruction> CreateParametersArrayInstructions(ILProcessor processor, MethodDefinition method, VariableDefinition arrayVariable /*parameters*/)
    {
        var createArray = new List<Instruction>
        {
            processor.Create(OpCodes.Ldc_I4, method.Parameters.Count), //method.Parameters.Count
            processor.Create(OpCodes.Newarr, objectTypeRef), // new object[method.Parameters.Count]
            processor.Create(OpCodes.Stloc, arrayVariable) // var objArray = new object[method.Parameters.Count]
        };

        foreach (var p in method.Parameters)
            createArray.AddRange(IlHelper.ProcessParam(p, arrayVariable));

        return createArray;
    }

    IEnumerable<Instruction> GetAttributeInstanceInstructions(
        ILProcessor processor,
        ICustomAttribute attribute,
        MethodDefinition method,
        VariableDefinition attributeVariableDefinition,
        VariableDefinition methodVariableDefinition,
        bool explicitMatch)
    {

        var getMethodFromHandleRef = referenceFinder.GetMethodReference(methodBaseTypeRef, md => md.Name == "GetMethodFromHandle" &&
                                                                                                  md.Parameters.Count == 2);

        var getTypeof = referenceFinder.GetMethodReference(systemTypeRef, md => md.Name == "GetTypeFromHandle");
        var ctor = referenceFinder.GetMethodReference(activatorTypeRef, md => md.Name == "CreateInstance" &&
                                                                               md.Parameters.Count == 1);

        var getCustomAttrs = referenceFinder.GetMethodReference(attributeTypeRef,
            md => md.Name == "GetCustomAttributes" &&
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
            processor.Create(OpCodes.Call, getMethodFromHandleRef), // Push method onto the stack, GetMethodFromHandle, result on stack
            processor.Create(OpCodes.Stloc, methodVariableDefinition), // Store method in __fody$method

            processor.Create(OpCodes.Nop),
        };

        if (explicitMatch &&
            method.CustomAttributes.Any(m => m.AttributeType.Equals(attribute.AttributeType)))
        {
            oInstructions.AddRange(new[]
            {
                processor.Create(OpCodes.Ldloc, methodVariableDefinition),
                processor.Create(OpCodes.Ldtoken, attribute.AttributeType),
                processor.Create(OpCodes.Call, getTypeof),
                processor.Create(OpCodes.Call, getCustomAttrs),

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
            oInstructions.AddRange(new[]
            {
                processor.Create(OpCodes.Ldtoken, method.DeclaringType),
                processor.Create(OpCodes.Call, getTypeof),
                processor.Create(OpCodes.Ldtoken, attribute.AttributeType),
                processor.Create(OpCodes.Call, getTypeof),
                processor.Create(OpCodes.Call, getCustomAttrs),

                processor.Create(OpCodes.Dup),
                processor.Create(OpCodes.Ldlen),
                processor.Create(OpCodes.Ldc_I4_1),
                processor.Create(OpCodes.Sub),

                processor.Create(OpCodes.Ldelem_Ref),

                processor.Create(OpCodes.Castclass, attribute.AttributeType),
                processor.Create(OpCodes.Stloc, attributeVariableDefinition),
            });
        }
        else
        {
            oInstructions.AddRange(new[]
            {
                processor.Create(OpCodes.Ldtoken, attribute.AttributeType),
                processor.Create(OpCodes.Call, getTypeof),
                processor.Create(OpCodes.Call, ctor),
                processor.Create(OpCodes.Castclass, attribute.AttributeType),
                processor.Create(OpCodes.Stloc, attributeVariableDefinition),
            });
        }

        return oInstructions;
    }

    static IEnumerable<Instruction> GetCallInitInstructions(
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

    static IEnumerable<Instruction> GetBypassInstructions(ILProcessor processor, VariableDefinition attributeVariableDefinition, MethodReference needBypassRef0, Instruction exit)
    {
        return new List<Instruction>
        {
            processor.Create(OpCodes.Ldloc, attributeVariableDefinition),
            processor.Create(OpCodes.Callvirt, needBypassRef0),
            processor.Create(OpCodes.Brtrue, exit),
        };
    }

    static IEnumerable<Instruction> GetCallOnEntryInstructions(
        ILProcessor processor,
        VariableDefinition attributeVariableDefinition,
        MethodReference onEntryMethodRef)
    {
        // Call __fody$attribute.OnEntry()
        return new List<Instruction>
        {
            processor.Create(OpCodes.Ldloc, attributeVariableDefinition),
            processor.Create(OpCodes.Callvirt, onEntryMethodRef),
        };
    }

    static IList<Instruction> GetSaveRetvalInstructions(ILProcessor processor, VariableDefinition retvalVariableDefinition)
    {
        var oInstructions = new List<Instruction>();
        if (retvalVariableDefinition != null && processor.Body.Instructions.Any(i => i.OpCode == OpCodes.Ret))
        {
            if (!retvalVariableDefinition.VariableType.IsValueType &&
                !retvalVariableDefinition.VariableType.IsGenericParameter)
            {
                oInstructions.Add(processor.Create(OpCodes.Castclass, retvalVariableDefinition.VariableType));
            }

            oInstructions.Add(processor.Create(OpCodes.Stloc, retvalVariableDefinition));
        }

        return oInstructions;
    }

    static IList<Instruction> GetCallOnExitInstructions(ILProcessor processor, VariableDefinition attributeVariableDefinition, MethodReference onExitMethodRef)
    {
        // Call __fody$attribute.OnExit()
        return new List<Instruction>
        {
            processor.Create(OpCodes.Ldloc, attributeVariableDefinition),
            processor.Create(OpCodes.Callvirt, onExitMethodRef)
        };
    }

    static IList<Instruction> GetCallOnExitInstructions(ILProcessor processor,
        VariableDefinition attributeVariableDefinition, MethodReference onExitMethodRef, VariableDefinition retvalVariableDefinition)
    {
        var oInstructions = new List<Instruction>
        {
            processor.Create(OpCodes.Ldloc, attributeVariableDefinition),
            processor.Create(OpCodes.Ldloc, retvalVariableDefinition),
        };

        if (retvalVariableDefinition.VariableType.IsValueType ||
            retvalVariableDefinition.VariableType.IsGenericParameter)
        {
            oInstructions.Add(processor.Create(OpCodes.Box, retvalVariableDefinition.VariableType));
        }

        oInstructions.Add(processor.Create(OpCodes.Callvirt, onExitMethodRef));
        return oInstructions;
    }

    static IList<Instruction> GetMethodBodyReturnInstructions(ILProcessor processor, VariableDefinition attributeVariableDefinition, VariableDefinition retvalVariableDefinition, MethodReference alterRetvalMethodRef)
    {
        var instructions = new List<Instruction>();
        if (retvalVariableDefinition != null)
        {
            if (alterRetvalMethodRef != null)
            {
                instructions.Add(processor.Create(OpCodes.Ldloc, attributeVariableDefinition));
                instructions.Add(processor.Create(OpCodes.Ldloc, retvalVariableDefinition));

                if (retvalVariableDefinition.VariableType.IsValueType ||
                    retvalVariableDefinition.VariableType.IsGenericParameter)
                {
                    instructions.Add(processor.Create(OpCodes.Box, retvalVariableDefinition.VariableType));
                }

                instructions.Add(processor.Create(OpCodes.Callvirt, alterRetvalMethodRef));
                instructions.Add(processor.Create(OpCodes.Unbox_Any, retvalVariableDefinition.VariableType));
            }
            else
            {
                instructions.Add(processor.Create(OpCodes.Ldloc, retvalVariableDefinition));
            }
        }

        instructions.Add(processor.Create(OpCodes.Ret));
        return instructions;
    }

    static IList<Instruction> GetTryCatchLeaveInstructions(ILProcessor processor, Instruction methodBodyReturnInstruction)
    {
        return new[] {processor.Create(OpCodes.Leave_S, methodBodyReturnInstruction)};
    }

    static List<Instruction> GetCatchHandlerInstructions(ILProcessor processor, VariableDefinition attributeVariableDefinition, VariableDefinition exceptionVariableDefinition, MethodReference onExceptionMethodRef)
    {
        // Store the exception in __fody$exception
        // Call __fody$attribute.OnException("{methodName}", __fody$exception)
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

    static void ReplaceRetInstructions(ILProcessor processor, Instruction methodEpilogueFirstInstruction)
    {
        // We cannot call ret inside a try/catch block. Replace all ret instructions with
        // an unconditional branch to the start of the OnExit epilogue
        var retInstructions = (from i in processor.Body.Instructions
            where i.OpCode == OpCodes.Ret
            select i).ToList();

        foreach (var instruction in retInstructions)
        {
            instruction.OpCode = OpCodes.Br;
            instruction.Operand = methodEpilogueFirstInstruction;
        }
    }

    static IEnumerable<Instruction> GetTaskContinuationInstructions(ILProcessor processor, VariableDefinition retvalVariableDefinition, VariableDefinition attributeVariableDefinition, MethodReference taskContinuationMethodReference)
    {
        if (retvalVariableDefinition != null)
        {
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