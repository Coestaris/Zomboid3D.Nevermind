using System.Reflection;

namespace NevermindCompiler.CLParser
{
    internal class InlineValue
    {
        public FieldInfo ClassField;
        public InlineValueAttribute Attribute;
        public string StringValue;

        public InlineValue(FieldInfo classField, InlineValueAttribute attribute)
        {
            ClassField = classField;
            Attribute = attribute;
        }
    }
}