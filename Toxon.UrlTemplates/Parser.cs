using System;
using System.Collections.Generic;
using System.Linq;

namespace Toxon.UrlTemplates
{
    internal class Parser
    {
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
            var literalResult = ParseLiteral(s);
            // var expressionResult = ParseExpression(s);

            return literalResult;
        }

        private ParserResult<UrlTemplateComponent> ParseLiteral(ParserState s)
        {
            return Read(s).Map(x =>
            {
                var c = x.Result;

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
                    return x.State.Success<UrlTemplateComponent>(new LiteralComponent(new string(new[] { c })));
                }

                // OR pct-encoded
                if (c != '%')
                {
                    return x.State.Failure<UrlTemplateComponent>("Didn't find any literal chars");
                }

                return Read(x.State, 2).Map(px =>
                {
                    if (!ParserUtils.IsHexDigit(px.Result[0]))
                    {
                        return px.State.Failure<UrlTemplateComponent>("Unexpected '{0}' in ParseLiteral following a pct symbol", px.Result[0]);
                    }
                    if (!ParserUtils.IsHexDigit(px.Result[1]))
                    {
                        return px.State.Failure<UrlTemplateComponent>("Unexpected '{0}' in ParseLiteral following a pct symbol", px.Result[1]);
                    }

                    return px.State.Success<UrlTemplateComponent>(new LiteralComponent($"%{px.Result}"));
                });
            });
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
    }
}
