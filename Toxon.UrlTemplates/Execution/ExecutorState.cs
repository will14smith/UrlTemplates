using System.Collections.Generic;
using System.Linq;
using Toxon.UrlTemplates.Model;

namespace Toxon.UrlTemplates.Execution
{
    internal class ExecutorState
    {
        public ExecutorState(ExpressionOperator op)
        {
            Operator = op;
            Result = new List<UrlTemplateComponent>();
        }

        private ExecutorState(ExpressionOperator op, IEnumerable<UrlTemplateComponent> result)
        {
            Operator = op;
            Result = result.ToList();
        }

        public ExpressionOperator Operator { get; }
        public IReadOnlyList<UrlTemplateComponent> Result { get; }

        public ExecutorState AddComponent(UrlTemplateComponent component)
        {
            return new ExecutorState(Operator, Result.Concat(new[] { component }));
        }

        public ExecutorState AddComponents(IEnumerable<UrlTemplateComponent> components)
        {
            return new ExecutorState(Operator, Result.Concat(components));
        }

        public ExecutorState WithNextOperator()
        {
            return new ExecutorState(ExpressionOperator.GetNextOperator(Operator), Result);
        }
    }
}