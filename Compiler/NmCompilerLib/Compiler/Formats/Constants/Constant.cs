using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.NMB;
using Nevermind.ByteCode.Types;
using Nevermind.ByteCode.Types.Scalar;
using Type = Nevermind.ByteCode.Types.Type;

namespace Nevermind.Compiler.Formats.Constants
{
    internal class Constant
    {
        public NmProgram Program;
        public Token CodeToken;

        public readonly ConstantType Type;

        public readonly long   IValue;
        public readonly double FValue;
        public readonly List<int> SValue;

        public readonly string Literal;

        private Constant(Token codeToken, NmProgram program, ConstantType type, string literal)
        {
            Type = type;
            Program = program;
            CodeToken = codeToken;
            Literal = literal;

            program.Constants.Add(this);
        }

        public Constant(Token codeToken, NmProgram program, long value, string literal) : this(codeToken, program, ConstantType.Integer, literal)
        {
            IValue = value;
        }

        public Constant(Token codeToken, NmProgram program, double value, string literal) : this(codeToken, program, ConstantType.Float, literal)
        {
            FValue = value;
        }

        public Constant(Token codeToken, NmProgram program, List<int> value) : this(codeToken, program, ConstantType.String, "")
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

        public Type ToProgramType()
        {
            switch (Type)
            {
                case ConstantType.Integer:
                {
                    if(string.IsNullOrEmpty(Literal))
                        return BuiltInTypes.DefaultConstInt32Type;

                    switch (Literal)
                    {
                        case "o": return BuiltInTypes.DefaultConstInt8Type;
                        case "uo": return BuiltInTypes.DefaultConstUInt8Type;

                        case "s": return BuiltInTypes.DefaultConstInt16Type;
                        case "us": return BuiltInTypes.DefaultConstUInt16Type;

                        case "u": return BuiltInTypes.DefaultConstUInt32Type;

                        case "l": return BuiltInTypes.DefaultConstInt64Type;
                        case "ul": return BuiltInTypes.DefaultConstUInt64Type;

                        default:
                            return null;
                    }
                }

                case ConstantType.Float:
                {
                    if(string.IsNullOrEmpty(Literal))
                        return BuiltInTypes.DefaultConstFloat64Type;

                    return BuiltInTypes.DefaultConstFloat32Type;
                }

                case ConstantType.String:
                    return BuiltInTypes.DefaultConstStringType;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal Variable ToVariable(NmProgram program)
        {
            return new Variable(ToProgramType(), "__const", -1, CodeToken, -1, VariableType.LinkToConst,
                program.Program.Header.GetConstIndex(this));
        }

        public override bool Equals(object obj)
        {
            if (obj is Constant)
            {
                var constant = (Constant)obj;
                if (Type == ConstantType.String && constant.Type == ConstantType.String)
                {
                    var firstNotSecond = constant.SValue.Except(SValue).ToList();
                    var secondNotFirst = SValue.Except(constant.SValue).ToList();
                    return !firstNotSecond.Any() && !secondNotFirst.Any();
                }
                else
                {
                    return Type == constant.Type &&
                        IValue == constant.IValue &&
                        FValue == constant.FValue;
                }
            }
            else return false;
        }

        public override int GetHashCode()
        {
            var hashCode = -531278061;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + IValue.GetHashCode();
            hashCode = hashCode * -1521134295 + FValue.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<int>>.Default.GetHashCode(SValue);
            return hashCode;
        }

        public List<byte> Serialize()
        {
            var programType = ToProgramType();
            switch (Type)
            {
                case ConstantType.Integer:
                    return (programType as IntegerType).Serialize(IValue);

                case ConstantType.Float:
                    return (programType as FloatType).Serialize(FValue);

                case ConstantType.String:
                    return (programType as StringType).Serialize(SValue);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}