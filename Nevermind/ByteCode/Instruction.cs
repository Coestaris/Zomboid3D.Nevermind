using System.Collections.Generic;

namespace Nevermind.ByteCode
{
    internal abstract class Instruction
    {
        public abstract List<byte> Serialize();
    }
}