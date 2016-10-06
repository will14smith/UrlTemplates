using System;
using Toxon.UrlTemplates.Values;

namespace Toxon.UrlTemplates.Resolvers
{
    public class ReflectionValueResolver : IValueResolver
    {
        private readonly object _value;
        private readonly Type _type;

        public ReflectionValueResolver(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _value = value;
            _type = value.GetType();
        }

        public IValue GetValue(string key)
        {
            var mapping = ReflectionMappings.GetTypeKeyMapping(_type, key);
            if (mapping == null)
            {
                // TODO more context
                throw new Exception("Failed to get a mapping");
            }

            return mapping(_value);
        }
    }
}
