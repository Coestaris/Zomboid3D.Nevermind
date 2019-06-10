using Nevermind.ByteCode.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nevermind.ByteCode.Instructions
{
    internal enum UnaryArithmeticIntsructionType
    {
        A_Neg,
        A_Not,
        A_BNeg,
    }

    internal class UnaryArithmeticIntsruction : ArithmeticIntsruction
    {
        public readonly Variable Operand;

        public readonly UnaryArithmeticIntsructionType Type;

        public override string InstructionName => Type.ToString().ToLower();

        public override int ParameterCount => 2;

        public override string SourceValue() =>ToSourceValue(Result?.Index ?? -1, Operand.ToSourceValue());

        public override List<byte> Serialize()
        {
            throw new System.NotImplementedException();
        }

        public UnaryArithmeticIntsruction(UnaryArithmeticIntsructionType type, Variable res, Variable a, Function func, ByteCode byteCode, int label) : base(res, func, byteCode, label)
        {
            Type = type;
            Operand = a;
        }
    }
}
