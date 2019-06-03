using System.Text.RegularExpressions;

namespace Nevermind.Compiler.Formats.Constants
{
    internal class DecConstantFormat : ConstantFormat
    {
        protected override Regex ConstantRegex { get; } = new Regex(@"^[0-9]*$");

        public override Constant Parse(Token input, NmProgram program)
        {
            return new Constant(input, program, long.Parse(input.StringValue));
        }
    }
}