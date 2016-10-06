using Toxon.UrlTemplates.Values;

namespace Toxon.UrlTemplates.Resolvers
{
    public class ConstantStringValue : IStringValue
    {
        public ConstantStringValue(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}