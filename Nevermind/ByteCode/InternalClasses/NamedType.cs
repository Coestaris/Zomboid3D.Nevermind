using Nevermind.ByteCode.Types;

namespace Nevermind.ByteCode.InternalClasses
{
    internal class NamedType
    {
        public string Name;
        public Type Type;

        public NamedType(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}