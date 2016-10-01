namespace Toxon.UrlTemplates
{
    internal class ExpressionOperator
    {
        public string Operator { get; }

        public ExpressionOperator(char opChar)
        {
            Operator = opChar.ToString();
        }

        public ExpressionOperator()
        {
            Operator = "";
        }

        public override string ToString()
        {
            return Operator;
        }
    }
}