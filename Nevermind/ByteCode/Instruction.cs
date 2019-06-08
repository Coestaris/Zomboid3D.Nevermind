using System.Collections.Generic;

namespace Nevermind.ByteCode
{
    internal abstract class Instruction
    {
        public abstract List<byte> Serialize(int label);

        public virtual string ToSource(string label)
        {
            return $"{label}: {ToString()}";
        }
    }
}