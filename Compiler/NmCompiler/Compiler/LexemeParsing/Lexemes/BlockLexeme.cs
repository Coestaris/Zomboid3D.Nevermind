namespace Nevermind.Compiler.LexemeParsing.Lexemes
{
    internal class BlockLexeme : Lexeme
    {
        public readonly Lexeme Parent;
        public readonly int Scope;

        public BlockLexeme(Lexeme parent, int scope) : base(null, LexemeType.Block, false)
        {
            Type = LexemeType.Block;
            Parent = parent;
            Scope = scope;
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