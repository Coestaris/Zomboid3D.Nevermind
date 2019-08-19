using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions.ArithmeticInstructions;
using Nevermind.ByteCode.NMB;

namespace Nevermind.ByteCode.Instructions.VectorInstructions
{
    internal class InstructionVget : ArithmeticInstruction
    {
        internal Variable _array;
        internal List<Variable> _indices;

        public override List<byte> Serialize() => Chunk.Int32ToBytes(Result.Index).ToList();

        public override InstructionType Type => InstructionType.Vget;

        public override string InstructionName => "vget";

        public override string SourceValue() => ToSourceValue(Result.ToSourceValue());

        public InstructionVget(Variable result, Variable array, List<Variable> indices, Function func, ByteCode byteCode,
            int label) : base(result, func, byteCode, label)
        {
            _array = array;
            _indices = indices;
        }
    }
}