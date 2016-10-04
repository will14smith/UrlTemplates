using System.Collections.Generic;
using System.Linq;

namespace Toxon.UrlTemplates.Values
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