using System.Collections.Generic;
using Nevermind.Compiler.LexemeParsing.Lexemes;

namespace Nevermind.Compiler.LexemeParsing
{
    internal abstract class ComplexLexeme : Lexeme
    {
        protected ComplexLexeme(List<Token> tokens, LexemeType type, bool requireBlock) : base(tokens, type, requireBlock) { }

        public BlockLexeme Block;
    }
}