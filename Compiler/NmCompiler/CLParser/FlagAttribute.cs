using System;

namespace NevermindCompiler.CLParser
{
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class FlagAttribute : Attribute
    {
        public string Name { get; set; }
        public char ShortName { get; set; }
        public string Description { get; set; }

        public FlagAttribute(string name, char shortName, string description)
        {
            Name = name;
            ShortName = shortName;
            Description = description;
        }
    }
}