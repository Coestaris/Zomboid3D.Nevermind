using System.Collections.Generic;

namespace Nevermind.Compiler.LexemeParsing.Lexemes
{
    internal class ModuleLexeme : Lexeme
    {
        public Token ModuleName;
        public bool IsLibrary;

        public ModuleLexeme(List<Token> tokens) : base(tokens, LexemeType.Module, false)
        {
            IsLibrary = tokens[0].Type == TokenType.LibraryKeyword;
            ModuleName = tokens[1];
        }
    }
}