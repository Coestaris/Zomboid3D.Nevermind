using System.Reflection;

namespace NevermindCompiler.CLParser
{
    internal class Flag
    {
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