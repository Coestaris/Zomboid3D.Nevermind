using System;
using System.Collections.Generic;

namespace Nevermind.Compiler.Formats
{
    internal class EscapeSymbol
    {
        public readonly bool HasCode;
        public readonly char Symbol;
        public readonly int Code;
        public readonly Func<string, int> StringCodeToByte;
        public readonly int MaxSymbols;

        public EscapeSymbol(char symbol, int code)
        {
            HasCode = false;
            Symbol = symbol;
            Code = code;
        }

        public EscapeSymbol(char symbol, Func<string, int> stringCodeToByte, int maxSymbols)
        {
            HasCode = true;
            Symbol = symbol;
            StringCodeToByte = stringCodeToByte;
            MaxSymbols = maxSymbols;
        }

        public static List<EscapeSymbol> EscapeSymbols = new List<EscapeSymbol>
        {
            new EscapeSymbol('a',  0x07),
            new EscapeSymbol('b',  0x08),
            new EscapeSymbol('f',  0x0C),
            new EscapeSymbol('n',  0x0A),
            new EscapeSymbol('r',  0x0D),
            new EscapeSymbol('t',  0x09),
            new EscapeSymbol('v',  0x0B),
            new EscapeSymbol('\\', 0x5C),
            new EscapeSymbol('"',  0x22),
            //Hex escape
            new EscapeSymbol('x', str => Convert.ToInt32(str, 16), 2),
            //Oct escape
            new EscapeSymbol('o', str => Convert.ToInt32(str, 8), 3),
            //Bin escape
            new EscapeSymbol('b', str => Convert.ToInt32(str, 2), 8),
            //Dec escape
            new EscapeSymbol('',  str => Convert.ToInt32(str), 3)
        };
    }

    internal static class StringFormat
    {
        public static bool CheckEscapeSymbols(string input, out List<int> charCodes)
        {

        }
    }
}