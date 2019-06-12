using Nevermind.ByteCode.Functions;
using System;
using System.Collections.Generic;

namespace Nevermind.ByteCode.Instructions.ArithmeticIntsructions
{
    internal class ArithmeticIntsruction : Instruction
    {
        public Variable Result;

        public override string InstructionName => throw new NotImplementedException();

        public override int ParameterCount => throw new NotImplementedException();

        public override List<byte> Serialize() => throw new NotImplementedException();

        public override string SourceValue() => throw new NotImplementedException();

        public ArithmeticIntsruction(Variable res, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            Result = res;
        }
    }
}
