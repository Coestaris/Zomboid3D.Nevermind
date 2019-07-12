using System;
using Nevermind.ByteCode.Functions;
using System.Collections.Generic;
using Nevermind.ByteCode.NMB;

namespace Nevermind.ByteCode.InternalClasses
{
    internal class FunctionInstructions
    {
        public List<Variable> Registers;
        public List<Variable> Locals;

        public readonly List<Instruction> Instructions;
        public readonly Function Function;

        public FunctionInstructions(Function function)
        {
            Function = function;
            Instructions = new List<Instruction>();
        }

        public Chunk GetChunk()
        {
            var ch = new Chunk(ChunkType.FUNC);
            ch.Add(Function.Index);
            ch.Add(Instructions.Count);

            ch.Add(Locals.Count);
            foreach (var local in Locals)
                ch.Add(Function.Program.ByteCode.Header.GetTypeIndex(local.Type));

            ch.Add(Registers.Count);
            foreach (var register in Registers)
                ch.Add(Function.Program.ByteCode.Header.GetTypeIndex(register.Type));

            foreach (var instruction in Instructions)
            {
                ch.Add(instruction.GetInstructionCode());
                ch.Add(instruction.Serialize());
            }

            return ch;
        }
    }
}