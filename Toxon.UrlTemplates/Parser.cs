using System;
using System.Collections.Generic;
using System.Linq;

namespace Toxon.UrlTemplates
{
    internal class Parser
    {
        public delegate ParserResult<TResult> ParseFn<TResult>(ParserState state);

        private readonly string _input;

        public Parser(string input)
        {
            _input = input;
        }

        public ParserResult<UrlTemplate> Parse()
        {
            var state = new ParserState();

            var components = new List<UrlTemplateComponent>();
            var errors = new List<ParserError>();

            while (!IsEof(state))
            {
                var result = ParseLiteralOrExpression(state);

                result.Match(
                    x => components.Add(x.Result),
                    x => errors.AddRange(x.Errors)
                );

                if (errors.Any())
                {
                    // TODO this could be handled better?
                    break;
                }

                state = result.State;
            }

            if (errors.Any())
            {
                return ParserResult.Failure<UrlTemplate>(state, errors);
            }

            return ParserResult.Success(state, new UrlTemplate(components));
        }

        private ParserResult<UrlTemplateComponent> ParseLiteralOrExpression(ParserState s)
        {
            return Alternative(ParseLiteral, ParseExpression)(s);
        }

        private ParserResult<UrlTemplateComponent> ParseLiteral(ParserState initialState)
        {
            return Read(initialState).Map(read =>
            {
                var c = read.Result;

                // %x21 / %x23-24 / %x26 / %x28-3B / %x3D / %x3F-5B / %x5D / %x5F / %x61-7A / %x7E
                // OR ucschar / iprivate
                if (c == 0x21
                || (c >= 0x23 && c <= 0x24)
                || c == 0x26
                || (c >= 0x28 && c <= 0x3B)
                || c == 0x3D
                || (c >= 0x3F && c <= 0x5B)
                || c == 0x5D
                || c == 0x5F
                || (c >= 0x61 && c <= 0x7A)
                || c == 0x7E
                || ParserUtils.IsUCSChar(c)
                || ParserUtils.IsIPrivate(c))
                {
                    // TODO escape NON-(reserved / unreserved / pct-encoded)
                    return read.State.Success<UrlTemplateComponent>(new LiteralComponent(new string(new[] { c })));
                }

                // OR pct-encoded
                // using the original state here because it reads the '%'
                return ParsePercentEncoded(initialState).Map(
                    percent => percent.State.Success<UrlTemplateComponent>(new LiteralComponent(percent.Result)),
                    percent => percent.State.Failure<UrlTemplateComponent>("Didn't find any literal chars"));
            });
        }

        private ParserResult<string> ParsePercentEncoded(ParserState initialState)
        {
            return Sequence(
                ReadOneOf('%'),
                ReadWhere(ParserUtils.IsHexDigit),
                ReadWhere(ParserUtils.IsHexDigit),
                (a, b, c) => $"{a}{b}{c}"
            )(initialState);
        }

        }

        private ParserResult<UrlTemplateComponent> ParseExpression(ParserState s)
        {
            throw new NotImplementedException();
        }

        #region state helpers

        private ParserResult<string> Read(ParserState s, int length)
        {
            if (IsEof(s, length - 1))
            {
                return s.Failure<string>("EOF");
            }

            return s.Advance(length).Success(_input.Substring(s.Position, length));
        }
        private ParserResult<char> Read(ParserState s)
        {
            if (IsEof(s, 0))
            {
                return s.Failure<char>("EOF");
            }

            return s.Advance(1).Success(_input[s.Position]);
        }

        private bool IsEof(ParserState s, int delta = 0)
        {
            return s.Position + delta >= _input.Length;
        }

        #endregion

        #region parsers 


        private ParseFn<char> ReadOneOf(params char[] allowedChars)
        {
            var allowedCharsSet = new HashSet<char>(allowedChars);

            return s =>
            {
                return Read(s).Map(read =>
                {
                    if (allowedCharsSet.Contains(read.Result))
                    {
                        return read.State.Success(read.Result);
                    }

                    var allowCharsString = string.Join(", ", allowedChars.Select(x => $"'{x}'"));
                    return read.State.Failure<char>($"Expecting one of: {allowCharsString}");
                });
            };
        }
        private ParseFn<char> ReadWhere(Func<char, bool> predicate)
        {
            return s => Read(s).Map(open => predicate(open.Result) ? open.State.Success(open.Result) : open.State.Failure<char>("Didn't match predicate"));
        }


        #endregion

        #region helpers

        private ParseFn<TResult> Optional<TResult>(ParseFn<TResult> parser)
        {
            // TODO should really be Option<TResult> return
            return state => parser(state).Map(x => x, x => x.State.Success(default(TResult)));
        }

        public ParseFn<TResult> Alternative<TResult>(params ParseFn<TResult>[] parsers)
        {
            return s =>
            {
                foreach (var parser in parsers)
                {
                    var result = parser(s);
                    if (result.IsSuccess)
                    {
                        return result;
                    }
                }

                return s.Failure<TResult>("Failed to match an alternative.");
            };
        }

        public ParseFn<TResult> Sequence<T1, T2, TResult>(ParseFn<T1> p1, ParseFn<T2> p2, Func<T1, T2, TResult> resultFn)
        {
            return initial => p1(initial).Map(r1 => p2(r1.State).Map(r2 => r2.State.Success(resultFn(r1.Result, r2.Result))));
        }
        public ParseFn<TResult> Sequence<T1, T2, T3, TResult>(ParseFn<T1> p1, ParseFn<T2> p2, ParseFn<T3> p3, Func<T1, T2, T3, TResult> resultFn)
        {
            var p12 = Sequence(p1, p2, Tuple.Create);

            return Sequence(p12, p3, (r12, r3) => resultFn(r12.Item1, r12.Item2, r3));
        }
        public ParseFn<TResult> Sequence<T1, T2, T3, T4, TResult>(ParseFn<T1> p1, ParseFn<T2> p2, ParseFn<T3> p3, ParseFn<T4> p4, Func<T1, T2, T3, T4, TResult> resultFn)
        {
            var p123 = Sequence(p1, p2, p3, Tuple.Create);

            return Sequence(p123, p4, (r123, r4) => resultFn(r123.Item1, r123.Item2, r123.Item3, r4));
        }

        #endregion
    }
}
