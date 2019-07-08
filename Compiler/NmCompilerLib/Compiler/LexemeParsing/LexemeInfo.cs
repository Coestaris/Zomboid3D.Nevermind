using System;
using System.Collections.Generic;
using Nevermind.Compiler.LexemeParsing.Lexemes;

namespace Nevermind.Compiler.LexemeParsing
{
    internal class LexemeInfo
    {
        public LexemeType Type;
        public List<LexemePatternToken> Pattern;

        public LexemeInfo(LexemeType type, List<LexemePatternToken> pattern)
        {
            Type = type;
            Pattern = pattern;
        }

        public override string ToString()
        {
            return $"{nameof(Type)}: {Type}";
        }

        public Lexeme CreateLexeme(List<Token> tokens, bool prototypesOnly)
        {
            if (prototypesOnly)
            {
                switch (Type)
                {
                    case LexemeType.Import:
                        return new ImportLexeme(tokens);
                    case LexemeType.Module:
                        return new ModuleLexeme(tokens);
                    case LexemeType.Function:
                        return new FunctionLexeme(tokens);
                    case LexemeType.Var:
                    case LexemeType.If:
                    case LexemeType.Expression:
                    case LexemeType.Return:
                    case LexemeType.Else:
                        return null;

                    case LexemeType.Block:
                    case LexemeType.Unknown:
                    default:
                        throw new ParseException(CompileErrorType.UnexpectedLexeme, tokens[0]);
                }
            }
            else
            {

                switch (Type)
                {
                    case LexemeType.Import:
                        return new ImportLexeme(tokens);
                    case LexemeType.Module:
                        return new ModuleLexeme(tokens);
                    case LexemeType.Var:
                        return new VarLexeme(tokens);
                    case LexemeType.If:
                        return new IfLexeme(tokens);
                    case LexemeType.Function:
                        return new FunctionLexeme(tokens);
                    case LexemeType.Expression:
                        return new ExpressionLexeme(tokens);
                    case LexemeType.Return:
                        return new ReturnLexeme(tokens);
                    case LexemeType.Else:
                        return new ElseLexeme(tokens);

                    case LexemeType.Block:
                    case LexemeType.Unknown:
                    default:
                        throw new ParseException(CompileErrorType.UnexpectedLexeme, tokens[0]);
                }
            }
        }
    }
}