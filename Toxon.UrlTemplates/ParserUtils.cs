using System;
using System.Linq;
using System.Text;

namespace Toxon.UrlTemplates
{
    internal class ParserUtils
    {
        public static bool IsAlpha(char c)
        {
            return (c >= 0x41 && c <= 0x5a)
                   || (c >= 0x61 && c <= 0x7a);
        }
        public static bool IsDigit(char c)
        {
            return c >= 0x30 && c <= 0x39;
        }
        public static bool IsHexDigit(char c)
        {
            return IsDigit(c)
                   || (c >= 0x41 && c <= 0x46)
                   || (c >= 0x61 && c <= 0x66);
        }

        public static bool IsPctEncoded(char c1, char c2)
        {
            return IsHexDigit(c1) && IsHexDigit(c2);
        }

        public static bool IsUnreserved(char c)
        {
            return IsAlpha(c)
                   || IsDigit(c)
                   || c == '-' || c == '.' || c == '_' || c == '~';
        }
        public static bool IsReserved(char c)
        {
            return IsGenDelim(c)
                   || IsSubDelim(c);
        }
        public static bool IsGenDelim(char c)
        {
            return c == ':' || c == '/' || c == '?' || c == '#'
                   || c == '[' || c == ']' || c == '@';
        }
        public static bool IsSubDelim(char c)
        {
            return c == '!' || c == '$' || c == '&' || c == '\''
                   || c == '(' || c == ')' || c == '*' || c == '+'
                   || c == ',' || c == ';' || c == '=';
        }

        public static bool IsUCSChar(char c)
        {
            return (c >= 0xA0 && c <= 0xD7FF)
                   || (c >= 0xF900 && c <= 0xFDCF)
                   || (c >= 0xFDF0 && c <= 0xFFEF)
                   || (c >= 0x10000 && c <= 0x1FFFD)
                   || (c >= 0x20000 && c <= 0x2FFFD)
                   || (c >= 0x30000 && c <= 0x3FFFD)
                   || (c >= 0x40000 && c <= 0x4FFFD)
                   || (c >= 0x50000 && c <= 0x5FFFD)
                   || (c >= 0x60000 && c <= 0x6FFFD)
                   || (c >= 0x70000 && c <= 0x7FFFD)
                   || (c >= 0x80000 && c <= 0x8FFFD)
                   || (c >= 0x90000 && c <= 0x9FFFD)
                   || (c >= 0xA0000 && c <= 0xAFFFD)
                   || (c >= 0xB0000 && c <= 0xBFFFD)
                   || (c >= 0xC0000 && c <= 0xCFFFD)
                   || (c >= 0xD0000 && c <= 0xDFFFD)
                   || (c >= 0xE0000 && c <= 0xEFFFD);

        }
        public static bool IsIPrivate(char c)
        {
            return (c >= 0xE000 && c <= 0xF8FF)
                   || (c >= 0xF0000 && c <= 0xFFFFD)
                   || (c >= 0x100000 && c <= 0x10FFFD);
        }

        public static string Escape(char input, Func<char, bool> allowedCharPredicate)
        {
            if (allowedCharPredicate(input))
            {
                return input.ToString();
            }

            var bytes = Encoding.UTF8.GetBytes(new[] { input });
            var result = new StringBuilder();

            foreach (var b in bytes)
            {
                result.Append('%');
                result.Append(BitConverter.ToString(new[] { b }));
            }

            return result.ToString();
        }
        public static string Escape(string input, Func<char, bool> allowedCharPredicate)
        {
            var result = new StringBuilder();

            foreach (var c in input)
            {
                result.Append(Escape(c, allowedCharPredicate));
            }

            return result.ToString();
        }
    }
}