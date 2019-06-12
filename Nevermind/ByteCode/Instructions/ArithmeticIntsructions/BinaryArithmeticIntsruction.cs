using Nevermind.ByteCode.Functions;
using System;
using System.Collections.Generic;

namespace Nevermind.ByteCode.Instructions.ArithmeticIntsructions
{
    internal enum BinaryArithmeticIntsructionType
    {
        A_Add,
        A_Sub,
        A_Mul,
        A_Div,
        A_lseq,
        A_ls,
        A_gr,
        A_greq,
        A_neq,
        A_eq,
        A_EDiv,
        A_LAnd,
        A_LOr,
        A_And,
        A_Xor,
        A_Or,
    }

    internal class BinaryArithmeticIntsruction : ArithmeticIntsruction
    {
        public readonly Variable Operand1;
        public readonly Variable Operand2;

        public readonly BinaryArithmeticIntsructionType Type;

        public override string InstructionName => Type.ToString().ToLower();

        public override int ParameterCount => 3;

        public override string SourceValue() =>
              ToSourceValue(Result?.Index ?? -1,
                  Operand1.ToSourceValue(),
                  Operand2.ToSourceValue());

        public override List<byte> Serialize()
        {
            throw new NotImplementedException();
        }

        public BinaryArithmeticIntsruction(BinaryArithmeticIntsructionType type, Variable res, Variable a, Variable b, Function func, ByteCode byteCode, int label) : base(res, func, byteCode, label)
        {
            Type = type;
            Operand1 = a;
            Operand2 = b;
        }
    }
}
