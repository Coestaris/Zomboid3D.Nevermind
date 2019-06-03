namespace Nevermind.Compiler.LexemeParsing
{
    internal class LexemePatternToken
    {
        public readonly bool IsRequired;
        public readonly TokenType Type;

        public LexemePatternToken(TokenType type, bool isRequired)
        {
            IsRequired = isRequired;
            Type = type;
        }
    }
}