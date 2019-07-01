using System.Text.RegularExpressions;

namespace Nevermind.Compiler.Formats.Constants
{
    internal abstract class ConstantFormat
    {
        protected abstract Regex ConstantRegex { get; }

        public virtual bool Verify(string input)
        {
            return ConstantRegex.IsMatch(input);
        }

        public abstract bool VerifyBounds(string input);

        public virtual Constant Parse(Token input, NmProgram program)
        {
            return new Constant(input, program, 0);
        }
    }
}