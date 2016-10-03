using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxon.UrlTemplates.Values;

namespace Toxon.UrlTemplates
{
    public class UrlTemplate
    {
        private readonly IReadOnlyList<UrlTemplateComponent> _components;

        internal UrlTemplate(IReadOnlyList<UrlTemplateComponent> components)
        {
            _components = components;
        }

        public UrlTemplate Resolve(IValueResolver valueResolver, bool allowPartial)
        {
            var result = new List<UrlTemplateComponent>();
            var sb = new StringBuilder();

            Action<UrlTemplateComponent> addComponent = component =>
            {
                if (component is LiteralComponent)
                {
                    sb.Append(((LiteralComponent)component).Value);
                }
                else
                {
                    if (sb.Length > 0)
                    {
                        result.Add(new LiteralComponent(sb.ToString()));
                        sb.Clear();
                    }

                    result.Add(component);
                }
            };

            foreach (var component in _components)
            {
                if (component is LiteralComponent)
                {
                    addComponent(component);
                }
                else
                {
                    var subcomps = ExpandExpression(valueResolver, (ExpressionComponent)component, allowPartial);
                    foreach (var subcomp in subcomps)
                    {
                        addComponent(subcomp);
                    }
                }
            }

            if (sb.Length > 0)
            {
                result.Add(new LiteralComponent(sb.ToString()));
            }

            return new UrlTemplate(result);
        }
        public string ResolveToString(IValueResolver valueResolver, bool allowPartial)
        {
            var result = Resolve(valueResolver, allowPartial);

            //TODO assert _components are all literals
            //TODO better string building

            return result.ToString();
        }

        private IReadOnlyList<UrlTemplateComponent> ExpandExpression(IValueResolver valueResolver, ExpressionComponent expression, bool allowPartial)
        {
            var result = new List<UrlTemplateComponent>();
            var op = expression.Operator;

            foreach (var varSpec in expression.Variables)
            {
                var value = valueResolver.GetValue(varSpec.Name);
                if (value == null || value is NullValue)
                {
                    if (allowPartial)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    result.Add(new LiteralComponent(op.Prefix));

                    if (value is IStringValue)
                    {
                        var strValue = ((IStringValue)value).Value;
                        if (op.Named)
                        {
                            result.Add(new LiteralComponent(Encode(varSpec.Name, op.EscapeMode)));

                            if (string.IsNullOrEmpty(strValue))
                            {
                                result.Add(new LiteralComponent(op.IfEmpty));
                            }
                            else
                            {
                                result.Add(new LiteralComponent("="));
                            }
                        }

                        var prefixLength = varSpec.GetPrefixLength();
                        if (prefixLength.HasValue && strValue.Length > prefixLength.Value)
                        {
                            //TODO count %XX as 1 char
                            strValue = strValue.Substring(0, prefixLength.Value);
                        }

                        result.Add(new LiteralComponent(Encode(strValue, op.EscapeMode)));
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                op = ExpressionOperator.GetNextOperator(op);
            }

            return result;
        }

        private string Encode(string input, ExpressionEscapeMode mode)
        {
            switch (mode)
            {
                case ExpressionEscapeMode.Unreserved:
                    return ParserUtils.Escape(input, ParserUtils.IsUnreserved);
                case ExpressionEscapeMode.UnreservedAndReserved:
                    return ParserUtils.Escape(input, c => ParserUtils.IsUnreserved(c) || ParserUtils.IsReserved(c));
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        public override string ToString()
        {
            return string.Join("", _components.Select(x => x.ToString()));
        }

        public static UrlTemplate Parse(string input)
        {
            // TODO better error reporting
            return new Parser(input).Parse().Map(
                x => x.Result,
                x => { throw new Exception("Failed to parse urltemplate"); });
        }
    }
}
