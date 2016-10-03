namespace Toxon.UrlTemplates.Values
{
    class ConstantStringValue : IStringValue
    {
        public ConstantStringValue(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}