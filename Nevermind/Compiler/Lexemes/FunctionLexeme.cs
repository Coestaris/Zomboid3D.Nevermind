using System.Collections.Generic;
using System.Linq;

namespace Nevermind.Compiler.Lexemes
{
    internal class FunctionLexeme : Lexeme
    {
        public Token Name;
        public BlockLexeme Block;
        public ExpressionLexeme Parameters;

        public FunctionLexeme(List<Token> tokens) : base(tokens, LexemeType.Function, true)
        {
            Name = tokens[1];
            Parameters = new ExpressionLexeme(tokens.Skip(3).Take(tokens.Count - 4).ToList());
        }
    }
}