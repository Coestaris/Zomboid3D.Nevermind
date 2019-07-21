using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using Nevermind.Compiler.Semantics.Attributes;

namespace Nevermind.Compiler.Semantics
{
    internal class AttributeInfo
    {
        public Regex Regex;
        public int ParametersCount;
        public AttributeType Type;

        public AttributeInfo(Regex regex, int parametersCount, AttributeType type)
        {
            Regex = regex;
            ParametersCount = parametersCount;
            Type = type;
        }
    }

    internal enum AttributeType
    {
        Syscall
    }

    internal abstract class Attribute
    {
        public Token Name;
        public List<Token> Parameters;
        public AttributeType Type;

        protected abstract CompileError VerifyParameters();

        internal static readonly List<AttributeInfo> Types = new List<AttributeInfo>
        {
            new AttributeInfo(new Regex("syscall"), 2, AttributeType.Syscall),
        };

        public static CompileError CreateAttribute(List<Token> tokens, out Attribute attribute)
        {
            attribute = null;

            if (tokens.Count == 0) return null;
            var parameters = new List<Token>();
            var name = tokens[0];

            var info = Types.Find(p => p.Regex.IsMatch(tokens[1].StringValue));

            if (info == null)
                return new CompileError(CompileErrorType.UnknownAttributeType, name);

            var iterator = new TokenIterator<Token>(tokens.Skip(3).Take(tokens.Count - 4));
            while (iterator.GetNext() != null)
            {
                if(iterator.Current.Type != TokenType.ComaSign)
                    parameters.Add(iterator.Current);
            }

            if(parameters.Count != info.ParametersCount && info.ParametersCount != -1)
                return new CompileError(CompileErrorType.WrongAttributeParameterCount, name);

            switch (info.Type)
            {
                case AttributeType.Syscall:
                    attribute = new SyscallAttribute(name, parameters);
                    break;
                default:
                    return new CompileError(CompileErrorType.UnknownAttributeType, name);
            }

            return attribute.VerifyParameters();
        }

        protected Attribute(AttributeType type, Token name, List<Token> parameters)
        {
            Name = name;
            Parameters = parameters;
        }
    }
}