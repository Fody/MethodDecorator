using System.Collections.Generic;

using Mono.Cecil.Cil;

namespace MethodDecorator.Fody
{
    public static class ILProcessorExtensions
    {
        public static void InsertBefore(this ILProcessor processor, Instruction target, IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions)
                processor.InsertBefore(target, instruction);
        }
    }
}