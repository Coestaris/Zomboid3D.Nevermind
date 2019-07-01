using System;
using System.Text.RegularExpressions;

namespace Nevermind.Compiler.Formats.Constants
{
    internal class FloatConstantFormat : ConstantFormat
    {
        protected override Regex ConstantRegex { get; } = new Regex(@"^([0-9]*\.[0-9]+|[0-9]+)$");

        public override Constant Parse(Token input, NmProgram program)
        {
            return new Constant(input, program, float.Parse(input.StringValue));
        }

        public override bool VerifyBounds(string input)
        {
            throw new NotImplementedException();
        }
    }
}