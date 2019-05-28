using System.Collections.Generic;

namespace Nevermind.Compiler.Lexemes.Expressions
{
    internal class ExpressionToken
    {
        public Token CodeToken;
        public List<ExpressionToken> SubTokens;
        public ExpressionToken Parent;

        public TokenType LOperator;
        public TokenType ROperator;


        public ExpressionToken(Token codeToken)
        {
            CodeToken = codeToken;
            SubTokens = new List<ExpressionToken>();
        }

        public override string ToString()
        {
            return CodeToken != null ? CodeToken.ToSource() : "<complex>";
        }
    }
}