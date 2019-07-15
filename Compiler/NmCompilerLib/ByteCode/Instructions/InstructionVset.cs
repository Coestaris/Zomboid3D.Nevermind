using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions.ArithmeticInstructions;
using Nevermind.ByteCode.NMB;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Instructions
{
    internal class InstructionVset : Instruction
    {
        public Variable Array;
        public Variable Src;
        public List<Variable> Indices;

        public override List<byte> Serialize()
        {
            var list = new List<byte>();
            list.AddRange(Chunk.Int32ToBytes(Array.Index));
            list.AddRange(Chunk.Int32ToBytes(Src.Index));
            list.AddRange(Chunk.Int32ToBytes(Indices.Count));
            foreach (var index in Indices)
                list.AddRange(index.Serialize());

            return list;
        }

        public override InstructionType Type => InstructionType.Vset;

        public override string InstructionName => "vset";

        public override int ParameterCount => 0;

        public override string SourceValue()
        {
            var list = new List<object>();
            list.Add(Array.ToSourceValue());
            list.Add(Src.ToSourceValue());
            list.AddRange(Indices.Select(p => p.ToSourceValue()));
            return ToSourceValue(list.ToArray());
        }

        public override bool UsesVariable(int index) =>
            Array.Index == index || Src.Index == index || Indices.Any(p => p.Index == index);

        public override List<Variable> FetchUsedVariables(int index)
        {
            var list = new List<Variable>();
            list.Add(Array);
            list.Add(Src);
            list.AddRange(Indices);
            return InnerFetch(index, list.ToArray());
        }

        public override void ReplaceRegisterUsage(int oldIndex, int newIndex)
        {
            if (Array.Index == oldIndex) Array = Array.Clone(newIndex);
            if (Src.Index == oldIndex) Src = Src.Clone(newIndex);
            for(var i = 0; i < Indices.Count; i++)
                if (Indices[i].Index == oldIndex) Indices[i] = Indices[i].Clone(newIndex);
        }

        public InstructionVset(Variable array, Variable src, List<Variable> indices, Function func, ByteCode byteCode,
            int label) : base(func, byteCode, label)
        {
            Array = array;
            Src = src;
            Indices = indices;
        }
    }
}