using Nevermind.ByteCode.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nevermind.ByteCode.Instructions
{
    internal enum BinaryArithmeticIntsructionType
    {
        A_Add,
        A_Mul,
        A_lseq,
        A_ls,
        A_gr,
        A_greq,
        A_neq,
        A_eq,
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
            throw new System.NotImplementedException();
        }

        public BinaryArithmeticIntsruction(BinaryArithmeticIntsructionType type, Variable res, Variable a, Variable b, Function func, ByteCode byteCode, int label) : base(res, func, byteCode, label)
        {
            Type = type;
            Operand1 = a;
            Operand2 = b;
        }
    }
}
