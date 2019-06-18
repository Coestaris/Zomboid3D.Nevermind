using System.Collections.Generic;
using System.Linq;
using Nevermind.ByteCode.Types;
using Nevermind.ByteCode.Functions;

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
                ReturnType = tokens[index].StringValue == "void" ? null : tokens[index];
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
                            throw new ParseException(CompileErrorType.UnexpectedToken, token);
                        parameterName = token;
                        state = 1;
                        break;
                    }

                    case 1:
                    {
                        if(token.Type != TokenType.Colon)
                            throw new ParseException(CompileErrorType.UnexpectedToken, token);
                        state = 2;
                        break;
                    }

                    case 2:
                    {
                        if(token.Type != TokenType.Identifier)
                            throw new ParseException(CompileErrorType.UnexpectedToken, token);
                        parameterType = token;
                        Parameters.Add(new LexemeFunctionParameter(parameterType, parameterName));
                        state = 3;
                        break;
                    }

                    case 3:
                    {
                        if(token.Type != TokenType.ComaSign)
                            throw new ParseException(CompileErrorType.UnexpectedToken, token);
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

            func = new Function(program, Name.StringValue, Modifier, returnType, Block.Scope, parameters, Block);
            return null;
        }
    }
}