using System.Collections.Generic;
using System.Text;
using Toxon.UrlTemplates.Model;
using Toxon.UrlTemplates.Utils;

namespace Toxon.UrlTemplates.Execution
{
    internal class ComponentOptimiser
    {
        private readonly IReadOnlyCollection<UrlTemplateComponent> _input;

        private readonly StringBuilder _literalBuffer = new StringBuilder();
        private Option<ExpressionOperator> _initialOperator = Option.None<ExpressionOperator>();
        private ExpressionOperator _nextOperator;
        private List<ExpressionVariable> _variableBuffer = new List<ExpressionVariable>();

        public ComponentOptimiser(IReadOnlyCollection<UrlTemplateComponent> input)
        {
            _input = input;
        }

        private void FlushAndClearBuffers(ICollection<UrlTemplateComponent> result)
        {
            if (_literalBuffer.Length > 0)
            {
                result.Add(new LiteralComponent(_literalBuffer.ToString()));

                _literalBuffer.Clear();
            }

            if (_initialOperator.HasValue)
            {
                result.Add(new ExpressionComponent(_initialOperator.Value, _variableBuffer));

                _initialOperator = Option.None<ExpressionOperator>();
                _nextOperator = null;
                _variableBuffer = new List<ExpressionVariable>();
            }

        }


        public IReadOnlyCollection<UrlTemplateComponent> Optimise()
        {
            var result = new List<UrlTemplateComponent>();

            UrlTemplateComponent previousComponent = null;
            foreach (var component in _input)
            {
                if (component is LiteralComponent)
                {
                    if (!(previousComponent is LiteralComponent))
                    {
                        FlushAndClearBuffers(result);
                    }

                    var literal = (LiteralComponent)component;
                    _literalBuffer.Append(literal.Value);
                }
                else
                {
                    if (!(previousComponent is ExpressionComponent))
                    {
                        FlushAndClearBuffers(result);
                    }

                    var expr = (ExpressionComponent)component;

                    if (_nextOperator == expr.Operator)
                    {
                        // append
                        _nextOperator = ExpressionOperator.GetNextOperator(_nextOperator);
                        _variableBuffer.AddRange(expr.Variables);
                    }
                    else
                    {
                        if (_initialOperator.HasValue)
                        {
                            // flush
                            FlushAndClearBuffers(result);
                        }

                        // start new 
                        _initialOperator = Option.Some(expr.Operator);
                        _nextOperator = ExpressionOperator.GetNextOperator(expr.Operator);
                        _variableBuffer.AddRange(expr.Variables);
                    }
                }

                previousComponent = component;
            }

            FlushAndClearBuffers(result);

            return result;

        }
    }
}
