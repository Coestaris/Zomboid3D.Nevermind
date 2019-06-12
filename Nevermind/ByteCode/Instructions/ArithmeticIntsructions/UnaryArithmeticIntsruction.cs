using Nevermind.ByteCode.Functions;
using System.Collections.Generic;

namespace Nevermind.ByteCode.Instructions.ArithmeticIntsructions
{
    internal class UnaryArithmeticIntsruction : ArithmeticIntsruction
    {
        public readonly Variable Operand;

        public readonly UnaryArithmeticIntsructionType Type;

        public override string InstructionName => Type.ToString().ToLower();

        public override int ParameterCount => 2;

        public override string SourceValue() => ToSourceValue(Result?.Index ?? -1, Operand.ToSourceValue());

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
