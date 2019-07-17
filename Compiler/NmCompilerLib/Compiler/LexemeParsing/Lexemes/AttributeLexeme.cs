using System.Collections.Generic;

namespace Nevermind.Compiler.LexemeParsing.Lexemes
{
    internal class AttributeLexeme : ComplexLexeme
    {
        public AttributeLexeme(List<Token> tokens) : base(tokens, LexemeType.Attribute, false) { }
    }
}