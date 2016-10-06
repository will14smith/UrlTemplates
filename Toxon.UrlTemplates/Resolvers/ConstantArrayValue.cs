using System.Collections.Generic;
using System.Linq;
using Toxon.UrlTemplates.Values;

namespace Toxon.UrlTemplates.Resolvers
{
    public class ConstantArrayValue : IArrayValue
    {
        private readonly IReadOnlyCollection<IStringValue> _values;

        public ConstantArrayValue(IEnumerable<IStringValue> values)
        {
            _values = values.ToList();
        }

        public IReadOnlyCollection<IStringValue> GetValues()
        {
            return _values;
        }
    }
}