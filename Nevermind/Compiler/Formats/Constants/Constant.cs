using System;
using System.Collections.Generic;
using System.Linq;

namespace Nevermind.Compiler.Formats.Constants
{
    internal class Constant
    {
        public NmProgram Program;
        public Token CodeToken;

        public readonly ConstantType Type;

        public readonly long   IValue;
        public readonly float  FValue;
        public readonly List<int> SValue;

        private Constant(Token codeToken, NmProgram program, ConstantType type)
        {
            Type = type;
            Program = program;
            CodeToken = codeToken;

            program.Constants.Add(this);
        }

        public Constant(Token codeToken, NmProgram program, long value) : this(codeToken, program, ConstantType.Integer)
        {
            IValue = value;
        }

        public Constant(Token codeToken, NmProgram program, float value) : this(codeToken, program, ConstantType.Float)
        {
            FValue = value;
        }

        public Constant(Token codeToken, NmProgram program, List<int> value) : this(codeToken, program, ConstantType.String)
        {
            SValue = value;
        }

        public TokenType ToTokenType()
        {
            switch (Type)
            {
                case ConstantType.Integer:
                    return TokenType.Number;
                case ConstantType.Float:
                    return TokenType.FloatNumber;
                case ConstantType.String:
                    return TokenType.StringToken;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string ToStringValue()
        {
            switch (Type)
            {
                case ConstantType.Integer:
                    return IValue.ToString();
                case ConstantType.Float:
                    return FValue.ToString();
                case ConstantType.String:
                    return string.Join("", SValue.Select(p => (char)p));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string ToString()
        {
            return $"{Type}: {ToStringValue()}";
        }
    }
}