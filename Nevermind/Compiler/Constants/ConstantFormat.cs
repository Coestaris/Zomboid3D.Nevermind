using System.Text.RegularExpressions;

namespace Nevermind.Compiler.Constants
{
    internal abstract class ConstantFormat
    {
        protected Regex ConstantRegex;

        public virtual bool Verify(string input)
        {
            return ConstantRegex.IsMatch(input);
        }

        public virtual Constant Parse(Token input, NmProgram program)
        {
            return new Constant(input, program, 0);
        }
    }
}