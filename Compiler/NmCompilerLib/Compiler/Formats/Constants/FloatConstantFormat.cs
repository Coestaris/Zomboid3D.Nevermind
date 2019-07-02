using System;
using System.Text.RegularExpressions;

namespace Nevermind.Compiler.Formats.Constants
{
    internal class FloatConstantFormat : ConstantFormat
    {
        protected override Regex ConstantRegex { get; } = new Regex(@"^([0-9]*\.[0-9]+|[0-9]+)f?$");

        public override Constant Parse(Token input, NmProgram program)
        {
            var literal = GetLiteral(input.StringValue);
            var number = input.StringValue.Substring(0, input.StringValue.Length - literal.Length);

            return new Constant(input, program, double.Parse(number), literal);
        }

        public override bool VerifyBounds(string input)
        {
            throw new NotImplementedException();
        }
    }
}