using System;
using System.Collections.Generic;
using System.Linq;

namespace Toxon.UrlTemplates
{
    public class UrlTemplate
    {
        private readonly IReadOnlyList<UrlTemplateComponent> _components;

        internal UrlTemplate(IReadOnlyList<UrlTemplateComponent> components)
        {
            _components = components;
        }

        public UrlTemplate Resolve(IParameterResolver parameterResolver)
        {
            throw new NotImplementedException();
        }
        public string ResolveToString(IParameterResolver parameterResolver, bool allowPartial)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Join("", _components.Select(x => x.ToString()));
        }
    }

    public interface IParameterResolver
    {
    }
}
