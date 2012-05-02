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

            var processor = method.Body.GetILProcessor();

            var originalFirstInstruction = method.Body.Instructions[0];

            var instructions = new List<Instruction>
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

            foreach (var instruction in instructions)
                processor.InsertBefore(originalFirstInstruction, instruction);

            var onEntryMethodRef = attribute.AttributeType.Resolve().Methods.First(md => md.Name == "OnEntry");
            var onExitMethodRef = attribute.AttributeType.Resolve().Methods.First(md => md.Name == "OnExit");

            var methodName = method.DeclaringType.FullName + "." + method.Name;

            instructions = new List<Instruction>
                           {
                               processor.Create(OpCodes.Ldloc_S, attributeVariableDefinition),
                               processor.Create(OpCodes.Ldstr, methodName),
                               processor.Create(OpCodes.Callvirt, onEntryMethodRef)
                           };
            foreach (var instruction in instructions)
                processor.InsertBefore(originalFirstInstruction, instruction);

            if (method.Name.Contains("Throw"))
                return;

            processor.Remove(method.Body.Instructions.Last());

            processor.Emit(OpCodes.Ldloc_S, attributeVariableDefinition);
            processor.Emit(OpCodes.Ldstr, methodName);
            processor.Emit(OpCodes.Callvirt, onExitMethodRef);
            processor.Emit(OpCodes.Ret);
        }
    }
}