using System.Collections.Generic;

namespace Nevermind.Compiler.Lexemes
{
    internal class BlockLexeme : Lexeme
    {
        public readonly Lexeme Parent;

        public BlockLexeme(Lexeme parent) : base(null, LexemeType.Block, false)
        {
            Type = LexemeType.Block;
            Parent = parent;
        }

        public override void Print(int level)
        {
            foreach (var childLexeme in ChildLexemes)
            {
                childLexeme.Print(level);
            }
        }
    }
}