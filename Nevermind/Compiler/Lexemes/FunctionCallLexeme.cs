using System.Collections.Generic;

namespace Nevermind.Compiler.Lexemes
{
    internal class FunctionCallLexeme : Lexeme
    {
        public Token Name;
        public ExpressionLexeme Expression;

        public FunctionCallLexeme(List<Token> tokens) : base(tokens, LexemeType.FunctionCall, false)
        {
        }

    }
}