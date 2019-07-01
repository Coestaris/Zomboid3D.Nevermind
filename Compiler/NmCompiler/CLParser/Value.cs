using System.Reflection;

namespace NevermindCompiler.CLParser
{
    internal class Value
    {
        public FieldInfo ClassField;
        public ValueAttribute Attribute;
        public string StringValue;

        public Value(FieldInfo classField, ValueAttribute attribute)
        {
            ClassField = classField;
            Attribute = attribute;
        }
    }
}