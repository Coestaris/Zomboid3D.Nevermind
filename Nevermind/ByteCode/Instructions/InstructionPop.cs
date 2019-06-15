using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions.ArithmeticIntsructions;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionPop : ArithmeticIntsruction
    {
        public override List<byte> Serialize()
        {
            throw new System.NotImplementedException();
        }

        public override string InstructionName => "pop";
        public override int ParameterCount => 0;
        public override string SourceValue() => ToSourceValue(Result.ToSourceValue());

        public InstructionPop(Variable result, Function func, ByteCode byteCode, int label) : base(result, func, byteCode, label) { }
    }
}