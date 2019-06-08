using System.Collections.Generic;

namespace Nevermind.ByteCode
{
    public class ByteCode
    {
        public NmProgram Program;
        internal ByteCodeHeader Header;
        internal List<Instruction> Instructions;

        public ByteCode(NmProgram program)
        {
            Program = program;
            Header = new ByteCodeHeader(program);
        }

        public string ToSource()
        {
            return Header.ToSource();
        }

        public byte[] ToBinary()
        {
            return Header.ToBinary();
        }
    }
}