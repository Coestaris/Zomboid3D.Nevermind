using System;
using System.Collections.Generic;

namespace Nevermind.Compiler.LexemeParsing
{
    internal class LexemeInfo
    {
        public LexemeType Type;
        public List<LexemePatternToken> Pattern;
        public Type LexemeType;

        public LexemeInfo(LexemeType type, List<LexemePatternToken> pattern, Type lexemeType)
        {
            Type = type;
            Pattern = pattern;
            LexemeType = lexemeType;
        }

        public override string ToString()
        {
            return $"{nameof(Type)}: {Type}";
        }
    }
}