using System.Collections.Generic;

namespace Toxon.UrlTemplates.Values
{
    public interface IArrayValue : IValue
    {
        IReadOnlyCollection<IStringValue> GetValues();
    }
}