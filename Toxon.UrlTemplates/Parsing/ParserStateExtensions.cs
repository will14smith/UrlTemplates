using System.Collections.Generic;

namespace Toxon.UrlTemplates.Parsing
{
    internal static class ParserStateExtensions
    {
        public static ParserResult<T> Success<T>(this ParserState state, T result)
        {
            return new ParserResult<T>.Success(state, result);
        }
        public static ParserResult<T> Failure<T>(this ParserState state, string message)
        {
            return new ParserResult<T>.Failure(state, new[] { new ParserError(message, state) });
        }
        public static ParserResult<T> Failure<T>(this ParserState state, string message, params object[] args)
        {
            return new ParserResult<T>.Failure(state, new[] { new ParserError(string.Format(message, args), state) });
        }
        public static ParserResult<T> Failure<T>(this ParserState state, IEnumerable<ParserError> errors)
        {
            return new ParserResult<T>.Failure(state, errors);
        }
    }
}