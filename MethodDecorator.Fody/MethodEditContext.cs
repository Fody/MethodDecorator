using System;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

#nullable enable

sealed class MethodEditContext : IDisposable
{
    private readonly MethodDefinition? _method;
    private readonly DebugInformationContext? _debugInformationContext;

    public MethodEditContext(MethodDefinition method)
    {
        if (method.HasBody)
        {
            _method = method;

            var methodBody = method.Body;

            var debugInformation = method.DebugInformation;
            if (debugInformation.HasSequencePoints)
            {
                _debugInformationContext = new DebugInformationContext(methodBody, debugInformation);
            }

            methodBody.SimplifyMacros();
        }
    }

    public void Dispose()
    {
        if (_method == null)
            return;

        _method.Body.OptimizeMacros();

        _debugInformationContext?.Update();
    }

    class DebugInformationContext
    {
        private readonly SequencePoint _entryPoint;
        private readonly Instruction _entryInstruction;
        private readonly MethodDebugInformation _debugInformation;
        private readonly MethodBody _methodBody;

        public DebugInformationContext(MethodBody methodBody, MethodDebugInformation debugInformation)
        {
            _methodBody = methodBody;
            _debugInformation = debugInformation;
            _entryInstruction = methodBody.Instructions[0];
            _entryPoint = debugInformation.SequencePoints[0];

            if (_entryPoint.Offset != 0)
            {
                throw new Fody.WeavingException("Invalid debug information detected. First sequence point must point to first instruction.");
            }
        }

        public void Update()
        {
            var instructions = _methodBody.Instructions;

            var entryInstruction = instructions[0];

            if (_entryInstruction != entryInstruction)
            {
                _debugInformation.SequencePoints[0] = _entryPoint.With(entryInstruction);
                _debugInformation.Scope.Start = new InstructionOffset(entryInstruction);
            }
        }
    }
}

static class ExtensionMethods
{
    public static SequencePoint With(this SequencePoint sequencePoint, Instruction instruction)
    {
        return new SequencePoint(instruction, sequencePoint.Document)
        {
            StartColumn = sequencePoint.StartColumn,
            EndColumn = sequencePoint.EndColumn,
            StartLine = sequencePoint.StartLine,
            EndLine = sequencePoint.EndLine
        };
    }
}

