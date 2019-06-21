using System.Text;

namespace Nevermind.Compiler
{
    public class CompileError
    {
        internal readonly Token Token;

        public string FileName => Token?.FileName;
        public int LineIndex => Token?.LineIndex ?? -1;
        public int CharIndex => Token?.LineOffset ?? -1;

        public CompileErrorType ErrorType;

        internal CompileError(CompileErrorType type)
        {
            ErrorType = type;
        }

        internal CompileError(CompileErrorType type, Token token)
        {
            ErrorType = type;
            Token = token;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Error: {0}", ErrorType);
            if(FileName != null)
                sb.AppendFormat(" at \"{0}\"", FileName);
            if (LineIndex != -1 && LineIndex != -1)
            {
                sb.AppendFormat(" in {0}:{1}", LineIndex, CharIndex);
            }
            else
            {
                if(CharIndex != -1)
                    sb.AppendFormat(" at char index {0}", CharIndex);
                if(LineIndex != -1)
                    sb.AppendFormat(" at line {0}", LineIndex);
            }
            return sb.ToString();
        }
    }
}