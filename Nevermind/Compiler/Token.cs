using System;
using System.Text.RegularExpressions;

namespace Nevermind.Compiler
{
    internal class Token
    {
        public TokenType Type;
        public string StringValue;

        private static readonly Regex numberRegex = new Regex(@"^[0-9]*$");
        private static readonly Regex floatNumberRegex = new Regex(@"^([0-9]*\.[0-9]*|[0-9]*)$");

        public Token(string str)
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

            StringValue = str;
        }

        public override string ToString()
        {
            if (Type == TokenType.Number || Type == TokenType.FloatNumber || Type == TokenType.Identifier)
                return $"Type: {Type}. Value: \"{StringValue}\"";

            return $"Type: {Type}";
        }
    }
}