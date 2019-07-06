﻿using System;
using Nevermind.Compiler;
using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.NMB;
using Type = Nevermind.ByteCode.Types.Type;

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

        public VariableType VariableType;

        public readonly int ConstIndex;
        public List<Variable> Tuple;

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

        public override string ToString()
        {
            return $"Variable \"{Name}\" - {ToSourceValue()}, at {Token}";
        }

        public IEnumerable<byte> Serialize()
        {
            var a = new List<byte>();
            if(VariableType == VariableType.Variable)
            {
                a.Add(0);
                a.AddRange(Chunk.Int32ToBytes(Index));
            }
            else if(VariableType == VariableType.LinkToConst)
            {
                a.Add(1);
                a.AddRange(Chunk.Int32ToBytes(ConstIndex));
            }
            else throw new ArgumentException("Variable should have Variable or LinkToConst type");
            return a;
        }

        public Variable Clone(int newIndex)
        {
            return new Variable(Type, Name, Scope, Token, newIndex, VariableType, ConstIndex);
        }
    }
}