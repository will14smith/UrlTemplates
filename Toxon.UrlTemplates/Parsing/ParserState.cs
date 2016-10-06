namespace Toxon.UrlTemplates.Parsing
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

        public override string ToString()
        {
            return "idx:" + Position;
        }
    }
}