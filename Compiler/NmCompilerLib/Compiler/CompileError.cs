using System.Text;

namespace Nevermind.Compiler
{
    public class CompileError
    {
        internal readonly Token Token;
        internal readonly Token DeclarationToken;

        public readonly CompileErrorType ErrorType;

        public string FileName => Token?.FileName;
        public int LineIndex => Token?.LineIndex ?? -1;
        public int CharIndex => Token?.LineOffset ?? -1;

        internal CompileError(CompileErrorType type, Token token = null, Token declarationToken = null)
        {
            ErrorType = type;
            Token = token;
            DeclarationToken = declarationToken;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Error: {0}", ErrorType);

            sb.Append(Token.ToErrorValue());

            if (DeclarationToken != null)
                sb.AppendFormat(" declared{0}", DeclarationToken.ToErrorValue());

            return sb.ToString();
        }
    }
}