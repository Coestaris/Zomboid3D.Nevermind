using Nevermind.ByteCode.Types;

namespace Nevermind.ByteCode.InternalClasses
{
    internal class NumeratedType
    {
        public int Index;
        public Type Type;

        public NumeratedType(int index, Type type)
        {
            Index = index;
            Type = type;
        }
    }
}