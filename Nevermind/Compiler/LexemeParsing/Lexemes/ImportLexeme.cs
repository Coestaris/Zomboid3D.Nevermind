using System.Collections.Generic;

namespace Nevermind.Compiler.LexemeParsing.Lexemes
{
    internal class ImportLexeme : Lexeme
    {
        public Token ImportName;

        public ImportLexeme(List<Token> tokens) : base(tokens, LexemeType.Import, false)
        {
            ImportName = tokens[1];
        }
    }
}