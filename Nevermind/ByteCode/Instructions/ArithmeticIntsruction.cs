using Nevermind.ByteCode.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nevermind.ByteCode.Instructions
{
    internal enum ArithmeticIntsructionType
    {
        Add,
        Mul,
    }

    internal class ArithmeticIntsruction : Instruction
    {
        public Variable Result;
        public readonly Variable Operand1;
        public readonly Variable Operand2;

        public readonly ArithmeticIntsructionType Type;

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

        public ArithmeticIntsruction(ArithmeticIntsructionType type, Variable res, Variable a, Variable b, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            Type = type;

            Result = res;
            Operand1 = a;
            Operand2 = b;
        }
    }
}
