namespace Nevermind.Compiler.Constants
{
    internal class Constant
    {
        public NmProgram Program;

        public ConstantType Type;

        public Token CodeToken;

        public long   IValue;
        public float  FValue;
        public string SValue;

        private Constant(Token codeToken, NmProgram program, ConstantType type)
        {
            Type = type;
            Program = program;
            CodeToken = codeToken;

            program.Constants.Add(this);
        }

        public Constant(Token codeToken, NmProgram program, long value) : this(codeToken, program, ConstantType.Integer)
        {
            IValue = value;
        }

        public Constant(Token codeToken, NmProgram program, float value) : this(codeToken, program, ConstantType.Float)
        {
            FValue = value;
        }

        public Constant(Token codeToken, NmProgram program, string value) : this(codeToken, program, ConstantType.String)
        {
            SValue = value;
        }
    }
}