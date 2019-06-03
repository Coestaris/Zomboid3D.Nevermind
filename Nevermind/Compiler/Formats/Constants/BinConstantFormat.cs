using System;
using System.Text.RegularExpressions;

namespace Nevermind.Compiler.Formats.Constants
{
    internal class BinConstantFormat : ConstantFormat
    {
        protected override Regex ConstantRegex { get; } = new Regex(@"^0[bB][01]+$");

        public override Constant Parse(Token input, NmProgram program)
        {
            var str = input.StringValue.ToLower().Split('b')[1];
            return new Constant(input, program, Convert.ToInt64(str, 2));
        }
    }
}