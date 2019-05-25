using System.Collections.Generic;

namespace Nevermind.Compiler.Lexemes
{
    internal abstract class Lexeme
    {
        public List<Token> Tokens;

        public Lexeme(List<Token> tokens)
        {
            this.Tokens = tokens;
        }

        public static List<LexemeInfo> Lexemes = new List<LexemeInfo>()
        {
            new LexemeInfo(LexemeType.Import, new List<TokenType>() { TokenType.ImportKeyword, TokenType.Identifier }, typeof(ImportLexeme))
        };
    }
}