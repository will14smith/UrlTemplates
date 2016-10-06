using System.Linq;
using Toxon.UrlTemplates.Utils;

namespace Toxon.UrlTemplates.Model
{
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

        public static bool HasExplode(this ExpressionVariable variable)
        {
            return variable.Modifier
                .OfType<ExpressionVariableModifier.Explode>()
                .Any();
        }
    }
}