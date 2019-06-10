using System;
using Nevermind.Compiler;

namespace Nevermind.ByteCode.Functions
{
    internal class Variable
    {
        public readonly int Scope;
        public readonly Type Type;
        public readonly string Name;
        public readonly int Index;

        public readonly Token Token;

        public readonly bool IsLinkToConst;
        public readonly int ConstIndex;

        public Variable(Type type, string name, int scope, Token token, int index, bool isLinkToConst = false, int constIndex = -1)
        {
            Type = type;
            Index = index;
            Name = name;
            Scope = scope;
            Token = token;

            IsLinkToConst = isLinkToConst;
            ConstIndex = constIndex;
        }

        internal string ToSourceValue()
        {
            return IsLinkToConst ? $"^{ConstIndex}" : Index.ToString();
        }
    }
}