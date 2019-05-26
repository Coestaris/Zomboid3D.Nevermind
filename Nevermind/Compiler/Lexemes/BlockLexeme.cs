using System.Collections.Generic;

namespace Nevermind.Compiler.Lexemes
{
    internal class BlockLexeme : Lexeme
    {
        public Lexeme Prev;
        public Lexeme Parent;

        public BlockLexeme(Lexeme parent, Lexeme prev) : base(null, LexemeType.Block)
        {
            Type = LexemeType.Block;
            Prev = Prev;
            Parent = parent;
        }
    }
}