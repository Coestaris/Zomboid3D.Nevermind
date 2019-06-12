using Nevermind.ByteCode.Functions;
using System;
using System.Collections.Generic;

namespace Nevermind.ByteCode.Instructions.ArithmeticIntsructions
{

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

        public bool CanBeSimplified()
        {
            return Type >= BinaryArithmeticIntsructionType.A_SetAdd &&
                   Type <= BinaryArithmeticIntsructionType.A_Set;
        }

        public Instruction Simplify()
        {
            switch (Type)
            {
                case BinaryArithmeticIntsructionType.A_SetAdd:
                    return new BinaryArithmeticIntsruction(BinaryArithmeticIntsructionType.A_Add, Operand1, Operand1, Operand2, Function, ByteCode, Label);
                case BinaryArithmeticIntsructionType.A_SetSub:
                    return new BinaryArithmeticIntsruction(BinaryArithmeticIntsructionType.A_Sub, Operand1, Operand1, Operand2, Function, ByteCode, Label);
                case BinaryArithmeticIntsructionType.A_SetMul:
                    return new BinaryArithmeticIntsruction(BinaryArithmeticIntsructionType.A_Mul, Operand1, Operand1, Operand2, Function, ByteCode, Label);
                case BinaryArithmeticIntsructionType.A_SetDiv:
                    return new BinaryArithmeticIntsruction(BinaryArithmeticIntsructionType.A_Div, Operand1, Operand1, Operand2, Function, ByteCode, Label);
                case BinaryArithmeticIntsructionType.A_SetEDiv:
                    return new BinaryArithmeticIntsruction(BinaryArithmeticIntsructionType.A_EDiv, Operand1, Operand1, Operand2, Function, ByteCode, Label);
                case BinaryArithmeticIntsructionType.A_SetAnd:
                    return new BinaryArithmeticIntsruction(BinaryArithmeticIntsructionType.A_And, Operand1, Operand1, Operand2, Function, ByteCode, Label);
                case BinaryArithmeticIntsructionType.A_SetXor:
                    return new BinaryArithmeticIntsruction(BinaryArithmeticIntsructionType.A_Xor, Operand1, Operand1, Operand2, Function, ByteCode, Label);
                case BinaryArithmeticIntsructionType.A_SetOr:
                    return new BinaryArithmeticIntsruction(BinaryArithmeticIntsructionType.A_Or, Operand1, Operand1, Operand2, Function, ByteCode, Label);
                case BinaryArithmeticIntsructionType.A_Set:
                    return new InstructionLdi(Operand2, Operand1, Function, ByteCode, Label);
            }
            return null;
        }
    }
}
