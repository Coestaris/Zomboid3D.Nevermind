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

        private readonly List<ConstantFormat> ConstantFormats = new List<ConstantFormat>
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

        public Token(string str, string fileName, int lineIndex, int lineOffset, NmProgram program, bool isString = false)
        {
            FileName = fileName;
            LineIndex = lineIndex;
            LineOffset = lineOffset;
            StringValue = str;

            if (!isString)
            {
                switch (str)
                {
                    case "=":
                        Type = TokenType.EqualSign;
                        break;
                    case "(":
                        Type = TokenType.BracketOpen;
                        break;
                    case ")":
                        Type = TokenType.BracketClosed;
                        break;
                    case "*":
                        Type = TokenType.MultiplySign;
                        break;
                    case "+":
                        Type = TokenType.PlusSign;
                        break;
                    case ";":
                        Type = TokenType.Semicolon;
                        break;
                    case ":":
                        Type = TokenType.Colon;
                        break;
                    case "{":
                        Type = TokenType.BraceOpened;
                        break;
                    case "}":
                        Type = TokenType.BraceClosed;
                        break;
                    case "if":
                        Type = TokenType.IfKeyword;
                        break;
                    case "var":
                        Type = TokenType.VarKeyword;
                        break;
                    case "function":
                        Type = TokenType.FunctionKeyword;
                        break;
                    case "import":
                        Type = TokenType.ImportKeyword;
                        break;
                    case "\"":
                        Type = TokenType.Quote;
                        break;

                    case "module":
                        Type = TokenType.ModuleKeyword;
                        break;
                    case "public":
                        Type = TokenType.PublicKeyword;
                        break;
                    case "private":
                        Type = TokenType.PrivateKeyword;
                        break;
                    case "entrypoint":
                        Type = TokenType.EntrypointKeyword;
                        break;
                    case "initialization":
                        Type = TokenType.InitializationKeyword;
                        break;
                    case "finalization":
                        Type = TokenType.FinalizationKeyword;
                        break;

                    case "-":
                        Type = TokenType.MinusSign;
                        break;
                    case "/":
                        Type = TokenType.DivideSign;
                        break;
                    case ">":
                        Type = TokenType.GreaterSign;
                        break;
                    case "<":
                        Type = TokenType.LessThanSign;
                        break;
                    case "~":
                        Type = TokenType.Tilda;
                        break;
                    case "&":
                        Type = TokenType.AmpersandSign;
                        break;
                    case "|":
                        Type = TokenType.OrSign;
                        break;
                    case "^":
                        Type = TokenType.CircumflexSign;
                        break;
                    case "%":
                        Type = TokenType.PercentSign;
                        break;
                    case "?":
                        Type = TokenType.QuestingSign;
                        break;
                    case "!":
                        Type = TokenType.ExclamationMark;
                        break;
                    case ",":
                        Type = TokenType.ComaSign;
                        break;

                    default:
                    {
                        if (LineIndex == -1)
                        {
                            Type = TokenType.Identifier;
                            break;
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
                            else throw new ParseException(this, CompileErrorType.WrongIdentifierFormat);
                        }
                    }
                        break;
                }
            }
            else
            {
                var chars = new List<int>();
                CompileErrorType error;
                if ((error = StringFormat.CheckEscapeSymbols(StringValue, out chars)) != 0)
                    throw new ParseException(this, error);

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