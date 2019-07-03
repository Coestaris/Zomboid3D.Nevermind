using System;
using System.Text.RegularExpressions;

namespace Nevermind.Compiler.Formats.Constants
{
    internal class DecConstantFormat : ConstantFormat
    {
        protected override Regex ConstantRegex { get; } = new Regex(@"^[0-9]+(o|uo|s|us|u|l|ul)?$");

        public override Constant Parse(Token input, NmProgram program)
        {
            var literal = GetLiteral(input.StringValue);
            var number = input.StringValue.Substring(0, input.StringValue.Length - literal.Length);

            return new Constant(input, program, long.Parse(number), literal);
        }

        public override bool VerifyBounds(Constant constant)
        {
            return CheckIntBound(constant);
        }
    }
}