using System.Collections.Generic;
using System.Linq;

namespace Toxon.UrlTemplates
{
    internal class ExpressionVariable
    {
        public string Name { get; }
        public IReadOnlyCollection<ExpressionVariableModifier> Modifier { get; }

        public ExpressionVariable(string name, IReadOnlyCollection<ExpressionVariableModifier> modifier)
        {
            Name = name;
            Modifier = modifier;
        }

        public override string ToString()
        {
            return Name + string.Join("", Modifier.Select(x => x.ToString()));
        }
    }
}