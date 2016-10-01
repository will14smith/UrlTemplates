namespace Toxon.UrlTemplates
{
    internal class ExpressionVariable
    {
        public string Name { get; }
        public ExpressionVariableModifier Modifier { get; }

        public ExpressionVariable(string name, ExpressionVariableModifier modifier)
        {
            Name = name;
            Modifier = modifier;
        }
    }

    internal class ExpressionVariableModifier
    {
        public class Prefix : ExpressionVariableModifier
        {
            public int Length { get; }

            public Prefix(int length)
            {
                Length = length;
            }
        }

        public class Explode : ExpressionVariableModifier
        {
        }
    }
}