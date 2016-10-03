using Toxon.UrlTemplates.Values;

namespace Toxon.UrlTemplates
{
    public interface IValueResolver
    {
        IValue GetValue(string key);
    }
}