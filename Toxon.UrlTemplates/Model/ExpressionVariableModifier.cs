namespace Toxon.UrlTemplates.Model
{
    internal class ExpressionVariableModifier
    {
        private ExpressionVariableModifier() { }

        public sealed class Prefix : ExpressionVariableModifier
        {
            public int Length { get; }

            public Prefix(int length)
            {
                Length = length;
            }

            public override string ToString()
            {
                return $":{Length}";
            }
        }

        public sealed class Explode : ExpressionVariableModifier
        {
            public override string ToString()
            {
                return "*";
            }
        }
    }
}