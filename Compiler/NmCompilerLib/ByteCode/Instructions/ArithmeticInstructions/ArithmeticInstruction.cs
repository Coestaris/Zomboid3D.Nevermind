using Nevermind.ByteCode.Functions;
using System;
using System.Collections.Generic;

namespace Nevermind.ByteCode.Instructions.ArithmeticInstructions
{
    internal class ArithmeticInstruction : Instruction
    {
        public Variable Result;

        public override string InstructionName
        {
            get { throw new NotImplementedException(); }
        }

        public override int ParameterCount
        {
            get { throw new NotImplementedException(); }
        }

        public override List<byte> Serialize()
        {
            throw new NotImplementedException();
        }

        public override string SourceValue()
        {
            throw new NotImplementedException();
        }

        public override bool UsesVariable(int index) => Result.Index == index;

        public override List<Variable> FetchUsedVariables(int index)
        {
            throw new NotImplementedException();
        }

        public override InstructionType Type
        {
            get { throw new NotImplementedException(); }
        }

        public ArithmeticInstruction(Variable res, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            Result = res;
        }
    }
}
