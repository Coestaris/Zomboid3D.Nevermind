using System;
using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode;
using Nevermind.ByteCode.Functions;
using Type = Nevermind.ByteCode.Type;

namespace Nevermind.Compiler.LexemeParsing.Lexemes
{
    internal class LexemeFunctionParameter
    {
        public Token Type;
        public Token Name;

        public LexemeFunctionParameter(Token type, Token name)
        {
            Type = type;
            Name = name;
        }
    }

    internal class FunctionLexeme : ComplexLexeme
    {
        public Token Name;
        public Token ReturnType;

        public BlockLexeme Block;
        public List<LexemeFunctionParameter> Parameters;

        public readonly FunctionModifier Modifier;

        public FunctionLexeme(List<Token> tokens) : base(tokens, LexemeType.Function, true)
        {
            switch (tokens[0].Type)
            {
                case TokenType.PublicKeyword:
                    Modifier = FunctionModifier.Public;
                    break;
                case TokenType.PrivateKeyword:
                    Modifier = FunctionModifier.Private;
                    break;
                case TokenType.EntrypointKeyword:
                    Modifier = FunctionModifier.Entrypoint;
                    break;
                case TokenType.FinalizationKeyword:
                    Modifier = FunctionModifier.Finalization;
                    break;
                case TokenType.InitializationKeyword:
                    Modifier = FunctionModifier.Initialization;
                    break;
                default:
                    Modifier = FunctionModifier.None;
                    break;
            }

            var index = 0;
            if (Modifier != FunctionModifier.None) index = 1;

            index++; //Function keyword;

            if (tokens[index + 1].Type == TokenType.Identifier)
            {
                //Has type and Name
                Name = tokens[index + 1];
                ReturnType = tokens[index];
                index += 2;
            }
            else
            {
                Name = tokens[index];
                index++;
            }

            index++; //bracket
            Token parameterName = null;
            Token parameterType = null;
            Parameters = new List<LexemeFunctionParameter>();
            var state = 0;

            foreach (var token in tokens.Skip(index).Take(tokens.Count - 1 - index))
            {
                switch (state)
                {
                    case 0:
                    {
                        if(token.Type != TokenType.Identifier)
                            throw new ParseException(token, CompileErrorType.UnexpectedToken);
                        parameterName = token;
                        state = 1;
                        break;
                    }

                    case 1:
                    {
                        if(token.Type != TokenType.Colon)
                            throw new ParseException(token, CompileErrorType.UnexpectedToken);
                        state = 2;
                        break;
                    }

                    case 2:
                    {
                        if(token.Type != TokenType.Identifier)
                            throw new ParseException(token, CompileErrorType.UnexpectedToken);
                        parameterType = token;
                        Parameters.Add(new LexemeFunctionParameter(parameterType, parameterName));
                        state = 3;
                        break;
                    }

                    case 3:
                    {
                        if(token.Type != TokenType.ComaSign)
                            throw new ParseException(token, CompileErrorType.UnexpectedToken);
                        state = 0;
                        break;
                    }
                }
            }
        }

        public override void Print(int level)
        {
            base.Print(level);
            Block?.Print(level + 1);
        }

        public Function ToFunc()
        {
            return new Function(
                Name.StringValue,
                Modifier,
                new Type(ReturnType.StringValue),
                Parameters.Select(p => new FunctionParameter(new Type(p.Type.StringValue), p.Name.StringValue)).ToList());
        }
    }
}