using Nevermind.ByteCode.Functions;
using System.Collections.Generic;
using Nevermind.ByteCode.NMB;

namespace Nevermind.ByteCode.InternalClasses
{
    internal class FunctionInstruction
    {
        public List<Variable> Registers;
        public List<Variable> Locals;

        public readonly List<Instruction> Instructions;
        public readonly Function Function;

        public FunctionInstruction(Function function)
        {
            Function = function;
            Instructions = new List<Instruction>();
        }

        public Chunk GetChunk()
        {
            var ch = new Chunk(ChunkType.FUNC);
            ch.Data.AddRange(Chunk.Int32ToBytes(Function.Index));
            ch.Data.AddRange(Chunk.Int32ToBytes(Instructions.Count));

            ch.Data.AddRange(Chunk.Int32ToBytes(Locals.Count));
            foreach (var local in Locals)
                ch.Data.AddRange(Chunk.Int32ToBytes(Function.Program.Program.Header.GetTypeIndex(local.Type)));

            ch.Data.AddRange(Chunk.Int32ToBytes(Registers.Count));
            foreach (var register in Registers)
                ch.Data.AddRange(Chunk.Int32ToBytes(Function.Program.Program.Header.GetTypeIndex(register.Type)));

            foreach (var instruction in Instructions)
            {
                ch.Data.AddRange(Chunk.UInt16ToBytes(instruction.GetInstructionCode()));
                ch.Data.AddRange(instruction.Serialize());
            }

            return ch;
        }
    }
}