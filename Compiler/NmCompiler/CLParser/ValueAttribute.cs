using System;

namespace NevermindCompiler.CLParser
{
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class ValueAttribute : Attribute
    {
        public string Name { get; set; }
        public char ShortName { get; set; }
        public string Description { get; set; }
        public object DefaultValue { get; set; }
        public bool CheckFileExistence { get; set; }
        public bool Required { get; set; }

        public ValueAttribute(string name, char shortName, string description, object defaultValue = null, bool required = false, bool checkFileExistence = false)
        {
            Name = name;
            ShortName = shortName;
            Description = description;
            CheckFileExistence = checkFileExistence;
            Required = required;
            DefaultValue = defaultValue;
        }
    }
}