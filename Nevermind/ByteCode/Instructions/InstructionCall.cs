using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionCall: Instruction
    {
        public Function DestFunc;

        public override List<byte> Serialize()
        {
            throw new System.NotImplementedException();
        }

        public override string InstructionName => "call";
        public override int ParameterCount => 0;
        public override string SourceValue() => ToSourceValue($"{DestFunc.Index}({DestFunc.Name})");

        public InstructionCall(Function destFunc, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            DestFunc = destFunc;
        }
    }
}