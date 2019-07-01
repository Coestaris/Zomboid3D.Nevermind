using System;

namespace NevermindCompiler.CLParser
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class HelpProviderAttribute : Attribute
    {
        public HelpProviderAttribute()
        {
        }
    }
}