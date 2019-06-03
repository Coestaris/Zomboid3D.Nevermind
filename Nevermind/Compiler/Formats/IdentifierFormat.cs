using System.Text.RegularExpressions;

namespace Nevermind.Compiler.Formats
{
    internal static class IdentifierFormat
    {
        private static Regex _identifierFormat = new Regex(@"[^\d][\w]*");

        public static bool Match(string input)
        {
            return _identifierFormat.IsMatch(input);
        }
    }
}