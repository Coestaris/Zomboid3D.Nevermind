using System;
using System.Text.RegularExpressions;

namespace Nevermind.Compiler.Formats.Constants
{
    internal class OctConstantFormat : ConstantFormat
    {
        protected override Regex ConstantRegex { get; } = new Regex(@"^0[0-7]+$");

        public override Constant Parse(Token input, NmProgram program)
        {
            return new Constant(input, program, Convert.ToInt64(input.StringValue, 8));
        }

        public override bool VerifyBounds(string input)
        {
            throw new NotImplementedException();
        }
    }
}