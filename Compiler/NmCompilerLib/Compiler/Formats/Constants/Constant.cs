using System;
using System.Collections.Generic;
using System.Linq;
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

        public Constant(Token codeToken, NmProgram program, double value) : this(codeToken, program, ConstantType.Float)
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

        public Type ToProgramType()
        {
            switch (Type)
            {
                case ConstantType.Integer:
                    return BuiltInTypes.DefaultConstIntType;
                case ConstantType.Float:
                    return BuiltInTypes.DefaultConstFloatType;
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