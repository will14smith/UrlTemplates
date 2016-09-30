namespace Toxon.UrlTemplates
{
    internal class LiteralComponent : UrlTemplateComponent
    {
        public string Value { get; }

        public LiteralComponent(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}