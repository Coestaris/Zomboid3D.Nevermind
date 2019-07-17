using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.Types;
using Nevermind.ByteCode.Functions;

namespace Nevermind.Compiler.LexemeParsing.Lexemes
{
    internal class LexemeFunctionParameter
    {
        public List<Token> Type;
        public Token Name;

        public LexemeFunctionParameter(List<Token> type, Token name)
        {
            Type = type;
            Name = name;
        }
    }

    internal class FunctionLexeme : ComplexLexeme
    {
        public Token Name;
        public List<Token> ReturnType;

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

            var bracketIndex = tokens.FindIndex(p => p.Type == TokenType.BracketOpen);

            if (bracketIndex != 2)
            {
                //Has type and Name
                Name = tokens[bracketIndex - 1];
                ReturnType =
                    tokens[index].StringValue == "void" ? null :
                    new List<Token>
                    {
                        tokens[index]

                    };

                index += 1;

                while (tokens[index].Type == TokenType.SquareBracketClosed ||
                       tokens[index].Type == TokenType.SquareBracketOpen)
                {
                    if(ReturnType == null)
                        throw new CompileException(CompileErrorType.UnexpectedToken, tokens[index]);

                    ReturnType.Add(tokens[index]);
                    index++;
                }

                index++;
            }
            else
            {
                Name = tokens[index];
                index++;
            }

            index++; //bracket
            Token parameterName = null;
            var parameterType = new List<Token>();

            Parameters = new List<LexemeFunctionParameter>();
            var state = 0;

            var tokenIterator = new TokenIterator<Token>(tokens.Skip(index).Take(tokens.Count - 1 - index));
            while(tokenIterator.GetNext() != null)
            {
                switch (state)
                {
                    case 0:
                    {
                        if(tokenIterator.Current.Type != TokenType.Identifier)
                            throw new CompileException(CompileErrorType.UnexpectedToken, tokenIterator.Current);

                        parameterName = tokenIterator.Current;
                        state = 1;
                        break;
                    }

                    case 1:
                    {
                        if(tokenIterator.Current.Type != TokenType.Colon)
                            throw new CompileException(CompileErrorType.UnexpectedToken, tokenIterator.Current);

                        state = 2;
                        break;
                    }

                    case 2:
                    {
                        if(tokenIterator.Current.Type != TokenType.Identifier)
                            throw new CompileException(CompileErrorType.UnexpectedToken, tokenIterator.Current);

                        parameterType.Add(tokenIterator.Current);

                        tokenIterator.GetNext();
                        while (tokenIterator.Current.Type == TokenType.SquareBracketOpen ||
                               tokenIterator.Current.Type == TokenType.SquareBracketClosed)
                        {
                            parameterType.Add(tokenIterator.Current);

                            if (tokenIterator.GetNext() == null)
                                break;
                        }

                        Parameters.Add(new LexemeFunctionParameter(parameterType, parameterName));
                        state = 3;
                        break;
                    }

                    case 3:
                    {
                        if(tokenIterator.Current.Type != TokenType.ComaSign)
                            throw new CompileException(CompileErrorType.UnexpectedToken, tokenIterator.Current);
                        state = 0;
                        break;
                    }
                }
            }
        }

        private CompileError GetParameterList(NmProgram program, out List<FunctionParameter> parameters)
        {
            parameters = new List<FunctionParameter>();
            CompileError error = null;

            foreach (var parameter in Parameters)
            {
                Type t;
                if ((error = ByteCode.Types.Type.GetType(program, parameter.Type, out t)) != null) return error;
                parameters.Add(new FunctionParameter(t, parameter.Name.StringValue, parameter.Name));
            }

            return error;
        }

        public CompileError ToFunc(NmProgram program, out Function func)
        {
            CompileError error;
            Type returnType = null;
            List<FunctionParameter> parameters;

            func = null;

            if (ReturnType != null)
                if ((error = ByteCode.Types.Type.GetType(program, ReturnType, out returnType)) != null) return error;
            if ((error = GetParameterList(program, out parameters)) != null) return error;

            func = new Function(program, Name.StringValue, Modifier, returnType, Block.Scope, parameters, Block)
            {
                Token = Name
            };
            return null;
        }
    }
}