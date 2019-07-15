using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions.ArithmeticInstructions;
using Nevermind.ByteCode.NMB;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionVget : ArithmeticInstruction
    {
        public Variable Array;
        public List<Variable> Indices;

        public override List<byte> Serialize()
        {
            var list = new List<byte>();
            list.AddRange(Chunk.Int32ToBytes(Result.Index));
            list.AddRange(Chunk.Int32ToBytes(Array.Index));
            list.AddRange(Chunk.Int32ToBytes(Indices.Count));
            foreach (var index in Indices)
                list.AddRange(index.Serialize());

            return list;
        }

        public override InstructionType Type => InstructionType.Vget;

        public override string InstructionName => "vget";

        public override int ParameterCount => 0;

        public override string SourceValue()
        {
            var list = new List<object>();
            list.Add(Result.ToSourceValue());
            list.Add(Array.ToSourceValue());
            list.AddRange(Indices.Select(p => p.ToSourceValue()));
            return ToSourceValue(list.ToArray());
        }

        public override bool UsesVariable(int index) =>
            base.UsesVariable(index) || Array.Index == index || Indices.Any(p => p.Index == index);

        public override List<Variable> FetchUsedVariables(int index)
        {
            var list = new List<Variable>();
            list.Add(Result);
            list.Add(Array);
            list.AddRange(Indices);
            return InnerFetch(index, list.ToArray());
        }

        public override void ReplaceRegisterUsage(int oldIndex, int newIndex)
        {
            base.ReplaceRegisterUsage(oldIndex, newIndex);
            if (Array.Index == oldIndex) Array = Array.Clone(newIndex);
            for(var i = 0; i < Indices.Count; i++)
                if (Indices[i].Index == oldIndex) Indices[i] = Indices[i].Clone(newIndex);
        }

        public InstructionVget(Variable result, Variable array, List<Variable> indices, Function func, ByteCode byteCode,
            int label) : base(result, func, byteCode, label)
        {
            Array = array;
            Indices = indices;
        }
    }
}