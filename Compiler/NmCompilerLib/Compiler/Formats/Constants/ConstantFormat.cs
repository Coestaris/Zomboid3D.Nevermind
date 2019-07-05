using System;
using System.Linq;
using System.Text.RegularExpressions;
using Nevermind.ByteCode.Types.Scalar;

namespace Nevermind.Compiler.Formats.Constants
{
    internal abstract class ConstantFormat
    {
        protected abstract Regex ConstantRegex { get; }

        protected string GetLiteral(string value)
        {
            char[] literals = { 'o', 'u', 's', 'l', 'f' };
            var result = "";
            var index = value.Length - 1;

            while (literals.Contains(value[index]))
                result += value[index--];

            return string.Join("", result.Reverse());
        }

        protected bool CheckIntBound(Constant constant)
        {
            var max = ((IntegerType)constant.ToProgramType()).Max();
            var min = ((IntegerType)constant.ToProgramType()).Min();

            return constant.IValue <= max && constant.IValue >= min;
        }

        protected bool CheckFloatBound(Constant constant)
        {
            if (constant.Literal == "f")
            {
                return constant.FValue < float.MaxValue &&
                       constant.FValue > float.MinValue;
            }

            return true;
        }

        public virtual bool Verify(string input)
        {
            return ConstantRegex.IsMatch(input);
        }

        public abstract bool VerifyBounds(Constant constant);

        public virtual Constant Parse(Token input, NmProgram program)
        {
            return null;
        }
    }
}