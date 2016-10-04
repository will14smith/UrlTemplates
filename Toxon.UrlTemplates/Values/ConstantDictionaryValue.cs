using System.Collections.Generic;

namespace Toxon.UrlTemplates.Values
{
    public class ConstantDictionaryValue : IDictionaryValue
    {
        private readonly IReadOnlyDictionary<string, IStringValue> _items;


        public ConstantDictionaryValue(IReadOnlyDictionary<string, IStringValue> items)
        {
            _items = items;
        }

        public IStringValue GetValue(string key)
        {
            return _items[key];
        }

        public IReadOnlyDictionary<string, IStringValue> GetValues()
        {
            return _items;
        }
    }
}