using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nevermind.Compiler.Formats
{
    internal class EscapeSymbolToken
    {
        public readonly char Symbol;
        public readonly int Code;

        public readonly Regex StringCodeValidation;
        public readonly bool HasCode;
        public readonly Func<string, int> StringCodeToByte;
        public readonly int MaxSymbols;

        public EscapeSymbolToken(char symbol, int code)
        {
            HasCode = false;
            Symbol = symbol;
            Code = code;
        }

        public EscapeSymbolToken(char symbol, Func<string, int> stringCodeToByte, int maxSymbols, Regex validation)
        {
            HasCode = true;
            Symbol = symbol;
            StringCodeToByte = stringCodeToByte;
            MaxSymbols = maxSymbols;
            StringCodeValidation = validation;
        }

        public static readonly char EscapeSymbol = '\\';

        public static List<EscapeSymbolToken> EscapeSymbols = new List<EscapeSymbolToken>
        {
            new EscapeSymbolToken('a',  0x07),
            new EscapeSymbolToken('b',  0x08),
            new EscapeSymbolToken('f',  0x0C),
            new EscapeSymbolToken('n',  0x0A),
            new EscapeSymbolToken('r',  0x0D),
            new EscapeSymbolToken('t',  0x09),
            new EscapeSymbolToken('v',  0x0B),
            new EscapeSymbolToken('\\', 0x5C),
            new EscapeSymbolToken('"',  0x22),
            //Hex escape
            new EscapeSymbolToken('x',
                str => Convert.ToInt32(str, 16), 2,
                new Regex(@"^[0-9a-zA-Z]+$")),
            //Oct escape
            new EscapeSymbolToken('o',
                str => Convert.ToInt32(str, 8), 3,
                new Regex(@"^[0-7]+$")),
            //Bin escape
            new EscapeSymbolToken('b',
                str => Convert.ToInt32(str, 2), 8,
                new Regex(@"^[0-1]+$")),
            //Dec escape
            new EscapeSymbolToken('\0',
                str => Convert.ToInt32(str), 3,
                new Regex(@"^[0-9]+$"))
        };
    }

    internal static class StringFormat
    {
        public static CompileErrorType CheckEscapeSymbols(string input, out List<int> charCodes)
        {
            charCodes = new List<int>();

            for(int i = 0; i < input.Length; i++)
            {
                if (input[i] == EscapeSymbolToken.EscapeSymbol)
                {
                    if (i == input.Length - 1)
                        return CompileErrorType.ExpectedEscapeSymbol;

                    int code = input[++i];

                    if (code > '0' && code < '9')
                    {
                        code = '\0';
                        i--;
                    }

                    var token = EscapeSymbolToken.EscapeSymbols.Find(p => p.Symbol == code);
                    if (token == null) return CompileErrorType.UnknownEscapeSymbol;

                    if (token.HasCode)
                    {
                        var stringCode = "";
                        while (stringCode.Length != token.MaxSymbols)
                        {
                            if (i == input.Length - 1)
                                return CompileErrorType.ExpectedEscapeSymbol;
                            stringCode += input[++i];
                        }

                        if (!token.StringCodeValidation.IsMatch(stringCode))
                            return CompileErrorType.WrongEscapeCodeFormat;

                        charCodes.Add(token.StringCodeToByte(stringCode));
                    }
                    else
                    {
                        charCodes.Add(token.Code);
                    }
                }
                else
                {
                    charCodes.Add(input[i]);
                }
            }
            return 0;
        }
    }
}