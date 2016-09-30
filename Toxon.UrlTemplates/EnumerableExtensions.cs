using System.Collections.Generic;
using System.Linq;

namespace Toxon.UrlTemplates
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> items, T item)
        {
            return items.Concat(new[] { item });
        }
    }
}

