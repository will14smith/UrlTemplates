using Toxon.UrlTemplates.Values;

namespace Toxon.UrlTemplates.Resolvers
{
    public interface IValueResolver
    {
        IValue GetValue(string key);
    }
}