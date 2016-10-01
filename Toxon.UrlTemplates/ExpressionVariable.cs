namespace Toxon.UrlTemplates
{
    internal class ExpressionVariable
    {
        public string Name { get; }
        public Option<ExpressionVariableModifier> Modifier { get; }

        public ExpressionVariable(string name, Option<ExpressionVariableModifier> modifier)
        {
            Name = name;
            Modifier = modifier;
        }

        public override string ToString()
        {
            return Name + Modifier.Map(x => x.ToString(), () => "");
        }
    }
}