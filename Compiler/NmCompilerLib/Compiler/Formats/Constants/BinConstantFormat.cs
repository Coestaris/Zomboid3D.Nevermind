using System;
using System.Text.RegularExpressions;

namespace Nevermind.Compiler.Formats.Constants
{
    internal class BinConstantFormat : ConstantFormat
    {
        protected override Regex ConstantRegex { get; } = new Regex(@"^0[bB][01]+(o|uo|s|us|u|l|ul)?$");

        public override Constant Parse(Token input, NmProgram program)
        {
            var literal = GetLiteral(input.StringValue);
            var number = input.StringValue.Substring(0, input.StringValue.Length - literal.Length);

            var str = number.ToLower().Split('b')[1];
            return new Constant(input, program, Convert.ToInt64(str, 2), literal);
        }

        public override bool VerifyBounds(Constant constant)
        {
            return CheckIntBound(constant);
        }
    }
}