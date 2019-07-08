using System.Collections.Generic;

namespace Nevermind.Compiler.LexemeParsing.Lexemes
{
    internal class ModuleLexeme : Lexeme
    {
        public Token ModuleName;

        public ModuleLexeme(List<Token> tokens) : base(tokens, LexemeType.Module, false)
        {
            ModuleName = tokens[1];
        }
    }
}