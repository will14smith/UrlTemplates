using System.Linq;

namespace Toxon.UrlTemplates
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

    internal static class ExpressionVariableModifierExtensions
    {
        public static Option<int> GetPrefixLength(this ExpressionVariable variable)
        {
            var prefix = variable.Modifier
                .OfType<ExpressionVariableModifier.Prefix>()
                .FirstOrDefault();

            if (prefix == null)
            {
                return Option.None<int>();
            }

            return Option.Some(prefix.Length);
        }
    }
}