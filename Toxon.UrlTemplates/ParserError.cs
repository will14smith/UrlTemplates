namespace Toxon.UrlTemplates
{
    public class ParserError
    {
        public string Message { get; }
        internal ParserState State { get; }

        internal ParserError(string message, ParserState state)
        {
            Message = message;
            State = state;
        }
    }
}