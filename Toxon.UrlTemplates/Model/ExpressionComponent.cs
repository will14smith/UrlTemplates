using System.Collections.Generic;

namespace Toxon.UrlTemplates.Model
{
    internal class ExpressionComponent : UrlTemplateComponent
    {
        public ExpressionOperator Operator { get; }
        public IReadOnlyList<ExpressionVariable> Variables { get; }

        public ExpressionComponent(ExpressionOperator op, IReadOnlyList<ExpressionVariable> variables)
        {
            Operator = op;
            Variables = variables;
        }

        public override string ToString()
        {
            return "{" + Operator + string.Join(",", Variables) + "}";
        }
    }
}