using System.Collections.Generic;

namespace Nevermind.Compiler.LexemeParsing.Lexemes
{
    internal class AttributeLexeme : Lexeme
    {
        public AttributeLexeme(List<Token> tokens) : base(tokens, LexemeType.Attribute, false) { }
    }
}