using System.Reflection;

namespace NevermindCompiler.CLParser
{
    internal class Flag
    {
        public const char NoName = '\0';

        public FieldInfo ClassField;
        public FlagAttribute Attribute;
        public bool Exists;

        public Flag(FieldInfo classField, FlagAttribute attribute)
        {
            ClassField = classField;
            Attribute = attribute;
        }
    }
}