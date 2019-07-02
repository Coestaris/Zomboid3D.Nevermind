using System.Linq;
using System.Text.RegularExpressions;

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

        public virtual bool Verify(string input)
        {
            return ConstantRegex.IsMatch(input);
        }

        public abstract bool VerifyBounds(string input);

        public virtual Constant Parse(Token input, NmProgram program)
        {
            return null;
        }
    }
}