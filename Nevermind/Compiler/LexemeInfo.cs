using System;
using System.Collections.Generic;

namespace Nevermind.Compiler.Lexemes
{
    internal class LexemeInfo
    {
        public LexemeType Type;
        public List<TokenType> TokenTypes;
        public Type LexemeType;

        public LexemeInfo(LexemeType type, List<TokenType> tokenTypes, Type lexemeType)
        {
            Type = type;
            TokenTypes = tokenTypes;
            LexemeType = lexemeType;
        }

        public override string ToString()
        {
            return $"{nameof(Type)}: {Type}";
        }
    }
}