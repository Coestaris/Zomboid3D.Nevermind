using System.Collections.Generic;

namespace Nevermind.Compiler.LexemeParsing.Lexemes
{
    internal class ElseLexeme : ComplexLexeme
    {
        public ElseLexeme(List<Token> tokens) : base(tokens, LexemeType.Else, true) { }
    }
}