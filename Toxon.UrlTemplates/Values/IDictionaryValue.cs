using System.Collections.Generic;
using System.Linq;

namespace Toxon.UrlTemplates.Values
{
    public interface IDictionaryValue : IValue
    {
        IStringValue GetValue(string key);
        IReadOnlyDictionary<string, IStringValue> GetValues();
    }
}