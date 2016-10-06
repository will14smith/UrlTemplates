using System;
using System.Collections.Generic;
using System.Linq;
using Toxon.UrlTemplates.Execution;
using Toxon.UrlTemplates.Model;
using Toxon.UrlTemplates.Parsing;

namespace Toxon.UrlTemplates
{
    public class UrlTemplate
    {
        private readonly IReadOnlyCollection<UrlTemplateComponent> _components;

        internal UrlTemplate(IReadOnlyCollection<UrlTemplateComponent> components)
        {
            _components = components;
        }

        public UrlTemplate Resolve(IValueResolver valueResolver, bool allowPartial)
        {
            return new Executor(_components, allowPartial, valueResolver).Execute();
        }
        public string ResolveToString(IValueResolver valueResolver, bool allowPartial)
        {
            var result = Resolve(valueResolver, allowPartial);

            //TODO assert _components are all literals
            //TODO better string building

            return result.ToString();
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
