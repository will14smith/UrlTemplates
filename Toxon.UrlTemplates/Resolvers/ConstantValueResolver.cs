using System;
using System.Collections.Generic;
using Toxon.UrlTemplates.Values;

namespace Toxon.UrlTemplates.Resolvers
{
    public class ConstantValueResolver : IValueResolver
    {
        private readonly IReadOnlyDictionary<string, IValue> _values;

        public ConstantValueResolver(IReadOnlyDictionary<string, IValue> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            _values = values;
        }

        public IValue GetValue(string key)
        {
            IValue result;

            if (_values.TryGetValue(key, out result))
            {
                return result;
            }

            return new NullValue();
        }
    }
}
