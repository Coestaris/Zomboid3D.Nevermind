using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionBrEq : Instruction
    {
        public int Index;
        public readonly Variable Variable;

        public override List<byte> Serialize()
        {
            throw new System.NotImplementedException();
        }

        public override string InstructionName => "breq";
        public override int ParameterCount => 2;
        public override string SourceValue() =>
            ToSourceValue(Variable.ToSourceValue(), $"_{Function.Name}" + Index);

        public InstructionBrEq(Variable variable, int index, Function func, ByteCode byteCode, int label) : base(func, byteCode, label)
        {
            if (variable.Type.ID != TypeID.Float && variable.Type.ID != TypeID.Integer)
                throw new ParseException(variable.Token, CompileErrorType.IncompatibleTypes);

            Variable = variable;
            Index = index;
        }
    }
}