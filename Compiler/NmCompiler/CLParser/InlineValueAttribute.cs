using System;

namespace NevermindCompiler.CLParser
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InlineValueAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool CheckFileExistence { get; set; }

        public InlineValueAttribute(string name, string description, bool checkFileExistence)
        {
            Name = name;
            Description = description;
            CheckFileExistence = checkFileExistence;
        }
    }
}