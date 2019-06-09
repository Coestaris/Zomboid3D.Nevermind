using System.Collections.Generic;
using Nevermind.ByteCode.Functions;

namespace Nevermind.ByteCode.Instructions.Arithmetic
{
    internal class InstructionAdd : Instruction
    {
        public override string InstructionName => "add";

        public Variable Result;
        public Variable Operand1;
        public Variable Operand2;

        public override int ParameterCount => 3;

        public override string SourceValue()
        {
            throw new System.NotImplementedException();
        }

        public override List<byte> Serialize()
        {
            throw new System.NotImplementedException();
        }

        public InstructionAdd(Variable res, Variable a, Variable b, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            Result = res;
            Operand1 = a;
            Operand2 = b;
        }
    }
}