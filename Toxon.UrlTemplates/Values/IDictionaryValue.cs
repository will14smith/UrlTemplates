using System.Collections.Generic;

namespace Toxon.UrlTemplates.Values
{
    public interface IDictionaryValue : IValue
    {
        IValue GetValue(string key);
        IReadOnlyDictionary<string, IValue> GetValues();
    }
}