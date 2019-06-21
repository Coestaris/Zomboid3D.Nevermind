using System.Collections.Generic;
using Nevermind.Compiler.Formats;
using Nevermind.Compiler.Formats.Constants;

namespace Nevermind.Compiler
{
    internal class Token
    {
        public string FileName;
        public int LineIndex;
        public int LineOffset;

        public TokenType Type;
        public string StringValue;

        public Constant Constant;

        private static readonly List<ConstantFormat> ConstantFormats = new List<ConstantFormat>
        {
            new OctConstantFormat(),
            new DecConstantFormat(),
            new HexConstantFormat(),
            new FloatConstantFormat(),
            new BinConstantFormat()
        };

        public const TokenType MathOperatorTokenType =
            TokenType.PlusSign      | TokenType.MinusSign       | TokenType.MultiplySign   | TokenType.DivideSign  |
            TokenType.EqualSign     | TokenType.GreaterSign     | TokenType.LessThanSign   | TokenType.Tilda       |
            TokenType.AmpersandSign | TokenType.OrSign          | TokenType.CircumflexSign | TokenType.PercentSign |
            TokenType.QuestingSign  | TokenType.ExclamationMark | TokenType.ComaSign;

        public const TokenType MathExpressionTokenType =
            MathOperatorTokenType   | TokenType.BracketClosed | TokenType.BracketOpen   | TokenType.Identifier  |
            TokenType.Number        | TokenType.FloatNumber   | TokenType.StringToken   | TokenType.ComplexToken;

        public const TokenType AnyTokenType =
            MathExpressionTokenType    | TokenType.ImportKeyword | TokenType.VarKeyword    | TokenType.IfKeyword  |
            TokenType.FunctionKeyword  | TokenType.Quote         | TokenType.Semicolon     | TokenType.Colon      |
            TokenType.BraceOpened      | TokenType.BraceClosed   | TokenType.ComplexToken;

        public Token(string fileName) : this("", fileName, -1, -1, null)
        {

        }

        private Dictionary<string, TokenType> _TokenDict = new Dictionary<string, TokenType>()
        {
            { "=" , TokenType.EqualSign },
            { "(", TokenType.BracketOpen },
            { ")", TokenType.BracketClosed },
            { "*", TokenType.MultiplySign },
            { "+", TokenType.PlusSign },
            { ";", TokenType.Semicolon },
            { ":", TokenType.Colon },
            { "{", TokenType.BraceOpened },
            { "}", TokenType.BraceClosed },
            { "if", TokenType.IfKeyword },
            { "var", TokenType.VarKeyword },
            { "function", TokenType.FunctionKeyword },
            { "import", TokenType.ImportKeyword },
            { "\"", TokenType.Quote },
            { "module", TokenType.ModuleKeyword },
            { "public", TokenType.PublicKeyword },
            { "private", TokenType.PrivateKeyword },
            { "entrypoint", TokenType.EntrypointKeyword },
            { "initialization", TokenType.InitializationKeyword },
            { "finalization", TokenType.FinalizationKeyword },
            { "-", TokenType.MinusSign },
            { "/", TokenType.DivideSign },
            { ">", TokenType.GreaterSign },
            { "<", TokenType.LessThanSign },
            { "~", TokenType.Tilda },
            { "&", TokenType.AmpersandSign },
            { "|", TokenType.OrSign },
            { "^", TokenType.CircumflexSign },
            { "%", TokenType.PercentSign },
            { "?", TokenType.QuestingSign },
            { "!", TokenType.ExclamationMark },
            { "return", TokenType.ReturnKeyword },
            { "else", TokenType.ElseKeyword },
            { ",", TokenType.ComaSign },
        };

        public Token(string str, string fileName, int lineIndex, int lineOffset, NmProgram program, bool isString = false)
        {
            FileName = fileName;
            LineIndex = lineIndex;
            LineOffset = lineOffset;
            StringValue = str;

            if (!isString)
            {
                if (!_TokenDict.TryGetValue(str, out Type))
                {
                    if (LineIndex == -1)
                    {
                        Type = TokenType.Identifier;
                    }

                    bool found = false;
                    foreach (var constantFormat in ConstantFormats)
                    {
                        if (constantFormat.Verify(StringValue))
                        {
                            Constant = constantFormat.Parse(this, program);
                            Type = Constant.ToTokenType();
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        if (IdentifierFormat.Match(StringValue)) Type = TokenType.Identifier;
                        else throw new ParseException(CompileErrorType.WrongIdentifierFormat, this);
                    }
                }
            }
            else
            {
                var chars = new List<int>();
                CompileErrorType error;
                if ((error = StringFormat.CheckEscapeSymbols(StringValue, out chars)) != 0)
                    throw new ParseException(error, this);

                Constant = new Constant(this, program, chars);

                Type = TokenType.StringToken;
            }
        }

        public override string ToString()
        {
            if (Type == TokenType.Number || Type == TokenType.FloatNumber || Type == TokenType.Identifier || Type == TokenType.StringToken)
                return $"\"{FileName}\" in {LineIndex}:{LineOffset}: Type: {Type}. Value: \"{StringValue}\"";

            return $"\"{FileName}\" in {LineIndex}:{LineOffset}: Type: {Type}";
        }

        public string ToSource()
        {
            return Type.ToSource(Type == TokenType.Number || Type == TokenType.FloatNumber || Type == TokenType.StringToken
                ? Constant.ToStringValue() : StringValue);
        }
    }
}