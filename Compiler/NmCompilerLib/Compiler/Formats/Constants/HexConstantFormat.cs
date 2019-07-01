using System;
using System.Text.RegularExpressions;

namespace Nevermind.Compiler.Formats.Constants
{
    internal class HexConstantFormat : ConstantFormat
    {
        protected override Regex ConstantRegex { get; } = new Regex(@"^0[xX][0-9a-fA-F]+$");

        public override Constant Parse(Token input, NmProgram program)
        {
            var str = input.StringValue.ToLower().Split('x')[1];
            return new Constant(input, program, Convert.ToInt64(str, 16));
        }

        public override bool VerifyBounds(string input)
        {
            throw new NotImplementedException();
        }
    }
}