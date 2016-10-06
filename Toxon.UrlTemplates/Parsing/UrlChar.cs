using System;
using System.Collections.Generic;
using System.Text;

namespace Toxon.UrlTemplates.Parsing
{
    internal abstract class UrlChar
    {
        public abstract T Match<T>(Func<Raw, T> raw, Func<PctEncoded, T> pctEncoded);

        public void Match(Action<Raw> raw, Action<PctEncoded> pctEncoded)
        {
            Match(x => { raw(x); return 0; }, x => { pctEncoded(x); return 0; });
        }

        public class Raw : UrlChar
        {
            public char Value { get; }

            public Raw(char value)
            {
                Value = value;
            }

            public override T Match<T>(Func<Raw, T> raw, Func<PctEncoded, T> pctEncoded)
            {
                return raw(this);
            }
        }
        public class PctEncoded : UrlChar
        {
            public char Value { get; }

            public PctEncoded(char value)
            {
                Value = value;
            }

            public override T Match<T>(Func<Raw, T> raw, Func<PctEncoded, T> pctEncoded)
            {
                return pctEncoded(this);
            }
        }

        public static IReadOnlyCollection<UrlChar> FromString(string input)
        {
            var result = new List<UrlChar>();
            var buffer = new List<byte>();

            for (var i = 0; i < input.Length; i++)
            {
                var c = input[i];

                if (c == '%' && input.Length > i + 2 && ParserUtils.IsHexDigit(input[i + 1]) && ParserUtils.IsHexDigit(input[i + 2]))
                {
                    buffer.Add(FromHex(input[i + 1], input[i + 2]));
                    i += 2;
                }
                else
                {
                    foreach (var x in Encoding.UTF8.GetString(buffer.ToArray()))
                    {
                        result.Add(new PctEncoded(x));
                    }
                    buffer.Clear();

                    result.Add(new Raw(input[i]));
                }
            }

            foreach (var x in Encoding.UTF8.GetString(buffer.ToArray()))
            {
                result.Add(new PctEncoded(x));
            }
            buffer.Clear();

            return result;
        }

        private static byte FromHex(char c)
        {
            if (c >= '0' && c <= '9') return (byte)(c - '0');
            if (c >= 'A' && c <= 'F') return (byte)(c - 'A' + 10);
            if (c >= 'a' && c <= 'f') return (byte)(c - 'a' + 10);

            throw new ArgumentOutOfRangeException();
        }
        private static byte FromHex(char c1, char c2)
        {
            return (byte)((FromHex(c1) << 4) | FromHex(c2));
        }
    }

}
