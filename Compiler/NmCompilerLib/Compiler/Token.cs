using System.Collections.Generic;
using System.IO;
using System.Text;
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
            MathOperatorTokenType         | TokenType.BracketClosed     | TokenType.BracketOpen | TokenType.Identifier   |
            TokenType.Number              | TokenType.FloatNumber       | TokenType.StringToken | TokenType.ComplexToken |
            TokenType.SquareBracketClosed | TokenType.SquareBracketOpen;

        public const TokenType AnyTokenType =
            MathExpressionTokenType     | TokenType.ImportKeyword         | TokenType.Quote                 | TokenType.Semicolon           |
            TokenType.Colon             | TokenType.BraceOpened           | TokenType.BraceClosed           | TokenType.ComplexToken        |
            TokenType.FunctionKeyword   | TokenType.ImportKeyword         | TokenType.IfKeyword             | TokenType.VarKeyword          |
            TokenType.ModuleKeyword     | TokenType.LibraryKeyword        | TokenType.PublicKeyword         | TokenType.PrivateKeyword      |
            TokenType.EntrypointKeyword | TokenType.InitializationKeyword | TokenType.FinalizationKeyword   | TokenType.ReturnKeyword       |
            TokenType.ElseKeyword;


        public Token(string fileName) : this("", fileName, -1, -1, null) { }

        private static readonly Dictionary<string, TokenType> TokenDict = new Dictionary<string, TokenType>
        {
            {"(", TokenType.BracketOpen},
            {")", TokenType.BracketClosed},
            {"{", TokenType.BraceOpened},
            {"}", TokenType.BraceClosed},
            {"[", TokenType.SquareBracketOpen},
            {"]", TokenType.SquareBracketClosed},
            {"-", TokenType.MinusSign},
            {"/", TokenType.DivideSign},
            {">", TokenType.GreaterSign},
            {"<", TokenType.LessThanSign},
            {"~", TokenType.Tilda},
            {"&", TokenType.AmpersandSign},
            {"=", TokenType.EqualSign},
            {"|", TokenType.OrSign},
            {"^", TokenType.CircumflexSign},
            {"%", TokenType.PercentSign},
            {"?", TokenType.QuestingSign},
            {"*", TokenType.MultiplySign},
            {"+", TokenType.PlusSign},
            {";", TokenType.Semicolon},
            {":", TokenType.Colon},
            {"!", TokenType.ExclamationMark},
            {",", TokenType.ComaSign},
            {"\"", TokenType.Quote},
            {"if", TokenType.IfKeyword},
            {"var", TokenType.VarKeyword},
            {"function", TokenType.FunctionKeyword},
            {"import", TokenType.ImportKeyword},
            {"module", TokenType.ModuleKeyword},
            {"library", TokenType.LibraryKeyword},
            {"public", TokenType.PublicKeyword},
            {"private", TokenType.PrivateKeyword},
            {"entrypoint", TokenType.EntrypointKeyword},
            {"initialization", TokenType.InitializationKeyword},
            {"finalization", TokenType.FinalizationKeyword},
            {"return", TokenType.ReturnKeyword},
            {"else", TokenType.ElseKeyword},
        };

        public Token(string str, string fileName, int lineIndex, int lineOffset, NmProgram program,
            bool isString = false)
        {
            FileName = fileName;
            LineIndex = lineIndex;
            LineOffset = lineOffset;
            StringValue = str;

            if (!isString)
            {
                if (!TokenDict.TryGetValue(str, out Type))
                {
                    if (LineIndex == -1)
                    {
                        Type = TokenType.Identifier;
                        return;
                    }

                    bool found = false;
                    foreach (var constantFormat in ConstantFormats)
                    {
                        if (constantFormat.Verify(StringValue))
                        {
                            Constant = constantFormat.Parse(this, program);

                            if (!constantFormat.VerifyBounds(Constant))
                                throw new CompileException(CompileErrorType.OutOfBoundsConstant, this);

                            Type = Constant.ToTokenType();
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        if (IdentifierFormat.Match(StringValue)) Type = TokenType.Identifier;
                        else throw new CompileException(CompileErrorType.WrongIdentifierFormat, this);
                    }
                }
            }
            else
            {
                var chars = new List<int>();
                CompileErrorType error;
                if ((error = StringFormat.CheckEscapeSymbols(StringValue, out chars)) != 0)
                    throw new CompileException(error, this);

                Constant = new Constant(this, program, chars);

                Type = TokenType.StringToken;
            }
        }

        public override string ToString()
        {
            if (Type == TokenType.Number || Type == TokenType.FloatNumber || Type == TokenType.Identifier ||
                Type == TokenType.StringToken)
                return $"\"{FileName}\" in {LineIndex}:{LineOffset}: Type: {Type}. Value: \"{StringValue}\"";

            return $"\"{FileName}\" in {LineIndex}:{LineOffset}: Type: {Type}";
        }

        public string ToSource()
        {
            return Type.ToSource(
                Type == TokenType.Number || Type == TokenType.FloatNumber || Type == TokenType.StringToken
                    ? Constant.ToStringValue()
                    : StringValue);
        }

        public string ToErrorValue()
        {
            var sb = new StringBuilder();

            if(FileName != null)
                sb.AppendFormat(" at \"{0}\"", new FileInfo(FileName).FullName);

            if (LineIndex != -1 && LineIndex != -1)
            {
                sb.AppendFormat(" in {0}:{1}", LineIndex, LineOffset);
            }
            else
            {
                if (LineIndex != -1)
                    sb.AppendFormat(" at char index {0}", LineOffset);
                if (LineIndex != -1)
                    sb.AppendFormat(" at line {0}", LineIndex);
            }

            return sb.ToString();
        }
    }
}