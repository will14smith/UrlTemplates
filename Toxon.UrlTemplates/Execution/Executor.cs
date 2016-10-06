using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxon.UrlTemplates.Model;
using Toxon.UrlTemplates.Parsing;
using Toxon.UrlTemplates.Values;

namespace Toxon.UrlTemplates.Execution
{
    internal class Executor
    {
        private readonly IReadOnlyCollection<UrlTemplateComponent> _components;
        private readonly bool _allowPartial;
        private readonly IValueResolver _valueResolver;

        public Executor(IReadOnlyCollection<UrlTemplateComponent> components, bool allowPartial, IValueResolver valueResolver)
        {
            _components = components;
            _allowPartial = allowPartial;
            _valueResolver = valueResolver;
        }

        public UrlTemplate Execute()
        {
            var result = new List<UrlTemplateComponent>();

            foreach (var component in _components)
            {
                if (component is LiteralComponent)
                {
                    result.Add(component);
                }
                else
                {
                    result.AddRange(ExpandExpression((ExpressionComponent)component));
                }
            }

            var optimisedResult = new ComponentOptimiser(result);

            return new UrlTemplate(optimisedResult.Optimise());
        }

        private IReadOnlyList<UrlTemplateComponent> ExpandExpression(ExpressionComponent expression)
        {
            var state = new ExecutorState(expression.Operator);

            foreach (var variable in expression.Variables)
            {
                var value = _valueResolver.GetValue(variable.Name);
                if (IsUndefined(value))
                {
                    if (_allowPartial)
                    {
                        state = state.AddComponent(new ExpressionComponent(state.Operator, new[] { variable }));
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    if (value is IStringValue)
                    {
                        state = ExpandString(state, variable, ((IStringValue)value).Value);
                    }
                    else if (value is IArrayValue)
                    {
                        state = ExpandArray(state, variable, ((IArrayValue)value).GetValues());
                    }
                    else if (value is IDictionaryValue)
                    {
                        state = ExpandDictionary(state, variable, ((IDictionaryValue)value).GetValues());
                    }
                    else
                    {
                        throw new InvalidOperationException("Unknown type?");
                    }
                }

                state = state.WithNextOperator();
            }

            return state.Result;
        }

        private ExecutorState ExpandString(ExecutorState state, ExpressionVariable variable, string value)
        {
            var op = state.Operator;

            state = state.AddComponent(new LiteralComponent(op.Prefix));

            if (op.Named)
            {
                state = state.AddComponent(new LiteralComponent(Encode(variable.Name, op.EscapeMode)));

                if (string.IsNullOrEmpty(value))
                {
                    state = state.AddComponent(new LiteralComponent(op.IfEmpty));
                    value = "";
                }
                else
                {
                    state = state.AddComponent(new LiteralComponent("="));
                }
            }

            var prefixLength = variable.GetPrefixLength();
            if (prefixLength.HasValue && value.Length > prefixLength.Value)
            {
                //TODO count %XX as 1 char
                value = value.Substring(0, prefixLength.Value);
            }

            return state.AddComponent(new LiteralComponent(Encode(value, op.EscapeMode)));
        }

        private ExecutorState ExpandArray(ExecutorState state, ExpressionVariable variable, IReadOnlyCollection<IStringValue> items)
        {
            var op = state.Operator;
            var first = true;

            if (!variable.HasExplode())
            {
                if (op.Named)
                {
                    state = state.AddComponent(new LiteralComponent(op.Prefix));
                    state = state.AddComponent(new LiteralComponent(Encode(variable.Name, op.EscapeMode)));

                    if (!items.Any())
                    {
                        state = state.AddComponent(new LiteralComponent(op.IfEmpty));
                    }
                    else
                    {
                        state = state.AddComponent(new LiteralComponent("="));
                    }
                }
                else if (items.Any())
                {
                    state = state.AddComponent(new LiteralComponent(op.Prefix));
                }

                foreach (var item in items)
                {
                    if (!first)
                    {
                        state = state.AddComponent(new LiteralComponent(","));
                    }
                    else
                    {
                        first = false;
                    }

                    state = state.AddComponent(new LiteralComponent(Encode(item.Value, op.EscapeMode)));
                }

                return state;
            }

            // exploded

            if (!op.Named)
            {
                foreach (var item in items)
                {
                    if (!first)
                    {
                        state = state.WithNextOperator();
                        op = state.Operator;
                    }
                    else
                    {
                        first = false;
                    }

                    state = state.AddComponent(new LiteralComponent(op.Prefix));
                    state = state.AddComponent(new LiteralComponent(Encode(item.Value, op.EscapeMode)));
                }

                return state;
            }

            // named exploded
            foreach (var item in items)
            {
                if (!first)
                {
                    state = state.WithNextOperator();
                    op = state.Operator;
                }
                else
                {
                    first = false;
                }

                state = state.AddComponent(new LiteralComponent(op.Prefix));
                if (op.Named)
                {
                    state = state.AddComponent(new LiteralComponent(Encode(variable.Name, op.EscapeMode)));

                    if (string.IsNullOrEmpty(item.Value))
                    {
                        state = state.AddComponent(new LiteralComponent(op.IfEmpty));
                    }
                    else
                    {
                        state = state.AddComponent(new LiteralComponent("="));
                    }
                }

                state = state.AddComponent(new LiteralComponent(Encode(item.Value, op.EscapeMode)));
            }

            return state;
        }
        private ExecutorState ExpandDictionary(ExecutorState state, ExpressionVariable variable, IReadOnlyDictionary<string, IStringValue> items)
        {
            var op = state.Operator;
            var first = true;

            if (!variable.HasExplode())
            {
                if (op.Named)
                {
                    state = state.AddComponent(new LiteralComponent(op.Prefix));
                    state = state.AddComponent(new LiteralComponent(Encode(variable.Name, op.EscapeMode)));

                    if (!items.Any())
                    {
                        state = state.AddComponent(new LiteralComponent(op.IfEmpty));
                    }
                    else
                    {
                        state = state.AddComponent(new LiteralComponent("="));
                    }
                }
                else if (items.Any())
                {
                    state = state.AddComponent(new LiteralComponent(op.Prefix));
                }

                foreach (var item in items)
                {
                    if (!first)
                    {
                        state = state.AddComponent(new LiteralComponent(","));
                    }
                    else
                    {
                        first = false;
                    }

                    state = state.AddComponent(new LiteralComponent(Encode(item.Key, op.EscapeMode)));
                    state = state.AddComponent(new LiteralComponent(","));
                    state = state.AddComponent(new LiteralComponent(Encode(item.Value.Value, op.EscapeMode)));
                }

                return state;
            }

            // explode

            if (!op.Named)
            {
                foreach (var item in items)
                {
                    if (!first)
                    {
                        state = state.WithNextOperator();
                        op = state.Operator;
                    }
                    else
                    {
                        first = false;
                    }

                    state = state.AddComponent(new LiteralComponent(op.Prefix));
                    state = state.AddComponent(new LiteralComponent(Encode(item.Key, op.EscapeMode)));
                    state = state.AddComponent(new LiteralComponent("="));
                    state = state.AddComponent(new LiteralComponent(Encode(item.Value.Value, op.EscapeMode)));
                }

                return state;
            }

            // named exploded
            foreach (var item in items)
            {
                if (!first)
                {
                    state = state.WithNextOperator();
                    op = state.Operator;
                }
                else
                {
                    first = false;
                }

                state = state.AddComponent(new LiteralComponent(op.Prefix));

                if (op.Named)
                {
                    state = state.AddComponent(new LiteralComponent(Encode(item.Key, op.EscapeMode)));

                    if (string.IsNullOrEmpty(item.Value.Value))
                    {
                        state = state.AddComponent(new LiteralComponent(op.IfEmpty));
                    }
                    else
                    {
                        state = state.AddComponent(new LiteralComponent("="));
                    }
                }

                state = state.AddComponent(new LiteralComponent(Encode(item.Value.Value, op.EscapeMode)));
            }

            return state;
        }

        private static bool IsUndefined(object value)
        {
            return value == null || value is NullValue;
        }

        private static string Encode(string input, ExpressionEscapeMode mode)
        {
            var urlString = UrlChar.FromString(input);

            var sb = new StringBuilder();
            Func<char, bool> allowChars;

            switch (mode)
            {
                case ExpressionEscapeMode.Unreserved:
                    allowChars = ParserUtils.IsUnreserved;
                    break;
                case ExpressionEscapeMode.UnreservedAndReserved:
                    allowChars = c => ParserUtils.IsUnreserved(c) || ParserUtils.IsReserved(c);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }

            foreach (var c in urlString)
            {
                c.Match(
                    raw: x => sb.Append(ParserUtils.Escape(x.Value, allowChars)),
                    pctEncoded: x => sb.Append(ParserUtils.Escape(x.Value, _ => false)));
            }

            return sb.ToString();
        }
    }
}
