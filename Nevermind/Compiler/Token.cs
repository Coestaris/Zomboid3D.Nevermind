using System;
using System.Text.RegularExpressions;

namespace Nevermind.Compiler
{
    internal class Token
    {
        public string FileName;
        public int LineIndex;
        public int LineOffset;

        public TokenType Type;
        public string StringValue;

        private static readonly Regex numberRegex = new Regex(@"^[0-9]*$");
        private static readonly Regex floatNumberRegex = new Regex(@"^([0-9]*\.[0-9]+|[0-9]+)$");

        public const TokenType MathExpressionTokenType =
            TokenType.Number | TokenType.FloatNumber | TokenType.BracketClosed | TokenType.BracketOpen |
            TokenType.EqualSign | TokenType.PlusSign | TokenType.MultiplySign | TokenType.ComplexToken |
            TokenType.Identifier;

        public const TokenType AnyTokenType =
            TokenType.ImportKeyword | TokenType.VarKeyword | TokenType.IfKeyword | TokenType.FunctionKeyword |
            TokenType.Identifier | TokenType.Number | TokenType.FloatNumber | TokenType.Quote |
            TokenType.Semicolon | TokenType.Colon | TokenType.BracketOpen | TokenType.BracketClosed |
            TokenType.EqualSign | TokenType.PlusSign | TokenType.MultiplySign | TokenType.BraceOpened |
            TokenType.BraceClosed | TokenType.ComplexToken;

        public Token(string str, string fileName, int lineIndex, int lineOffset)
        {
            switch (str)
            {
                case "=": Type = TokenType.EqualSign;
                    break;
                case "(": Type = TokenType.BracketOpen;
                    break;
                case ")": Type = TokenType.BracketClosed;
                    break;
                case "*": Type = TokenType.MultiplySign;
                    break;
                case "+": Type = TokenType.PlusSign;
                    break;
                case ";": Type = TokenType.Semicolon;
                    break;
                case ":": Type = TokenType.Colon;
                    break;
                case "{": Type = TokenType.BraceOpened;
                    break;
                case "}": Type = TokenType.BraceClosed;
                    break;
                case "if": Type = TokenType.IfKeyword;
                    break;
                case "var": Type = TokenType.VarKeyword;
                    break;
                case "function": Type = TokenType.FunctionKeyword;
                    break;
                case "import": Type = TokenType.ImportKeyword;
                    break;
                case "\"": Type = TokenType.Quote;
                    break;

                default:
                {
                    if (numberRegex.IsMatch(str)) Type = TokenType.Number;
                    else if (floatNumberRegex.IsMatch(str)) Type = TokenType.FloatNumber;
                    else Type = TokenType.Identifier;
                }
                    break;

            }

            FileName = fileName;
            LineIndex = lineIndex;
            LineOffset = lineOffset;
            StringValue = str;
        }

        public override string ToString()
        {
            if (Type == TokenType.Number || Type == TokenType.FloatNumber || Type == TokenType.Identifier)
                return $"\"{FileName}\" in {LineIndex}:{LineOffset}: Type: {Type}. Value: \"{StringValue}\"";

            return $"\"{FileName}\" in {LineIndex}:{LineOffset}: Type: {Type}";
        }

        public string ToSource()
        {
            switch (Type)
            {
                case TokenType.ImportKeyword:
                    return "import";
                case TokenType.VarKeyword:
                    return "var";
                case TokenType.IfKeyword:
                    return "if";
                case TokenType.FunctionKeyword:
                    return "function";
                case TokenType.Identifier:
                    return $"<identifier:{StringValue}>";
                case TokenType.Number:
                    return $"<number:{StringValue}>";
                case TokenType.FloatNumber:
                    return $"<float:{StringValue}>";
                case TokenType.Quote:
                    return "\"";
                case TokenType.Semicolon:
                    return ";";
                case TokenType.Colon:
                    return ":";
                case TokenType.BracketOpen:
                    return "(";
                case TokenType.BracketClosed:
                    return ")";
                case TokenType.EqualSign:
                    return "=";
                case TokenType.PlusSign:
                    return "+";
                case TokenType.MultiplySign:
                    return "*";
                case TokenType.BraceOpened:
                    return "{";
                case TokenType.BraceClosed:
                    return "}";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}