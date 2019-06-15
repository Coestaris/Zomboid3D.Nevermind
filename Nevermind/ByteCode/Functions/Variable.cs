using Nevermind.Compiler;
using Nevermind.ByteCode.Types;
using System.Collections.Generic;
using System.Linq;

namespace Nevermind.ByteCode.Functions
{
    internal enum VariableType
    {
        Variable,
        LinkToConst,
        Tuple,
    }

    internal class Variable
    {
        public readonly int Scope;
        public readonly Type Type;
        public readonly string Name;
        public int Index;

        public readonly Token Token;

        public readonly VariableType VariableType;

        public readonly int ConstIndex;
        public readonly List<Variable> Tuple;

        public Variable(Type type, string name, int scope, Token token, int index, VariableType variableType, int constIndex = -1)
        {
            Type = type;
            Index = index;
            Name = name;
            Scope = scope;
            Token = token;

            VariableType = variableType;

            if (variableType == VariableType.LinkToConst)
                ConstIndex = constIndex;
            else if (variableType == VariableType.Tuple)
                Tuple = new List<Variable>();
        }

        internal string ToSourceValue()
        {
            if (VariableType == VariableType.Variable)
                return Index.ToString();
            else if (VariableType == VariableType.LinkToConst)
                return $"^{ConstIndex}";
            else
                return $"tuple({string.Join(", ", Tuple.Select(p => p.ToSourceValue()))})";
        }
    }
}