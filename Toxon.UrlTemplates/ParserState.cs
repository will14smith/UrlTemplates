namespace Toxon.UrlTemplates
{
    internal class ParserState
    {
        public ParserState()
        {
            Position = 0;
        }

        private ParserState(int position)
        {
            Position = position;
        }

        public int Position { get; }
        public ParserState Advance(int i)
        {
            return new ParserState(Position + i);
        }

        public ParserResult<T> Success<T>(T result)
        {
            return ParserResult.Success(this, result);
        }
        public ParserResult<T> Failure<T>(string message, params object[] args)
        {
            return ParserResult.Failure<T>(this, new[] { new ParserError(string.Format(message, args), this) });
        }
    }
}