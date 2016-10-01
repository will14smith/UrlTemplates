using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Toxon.UrlTemplates.UnitTests
{
    public partial class ParserTests
    {
        private ParserResult<UrlTemplate> Parse(string input)
        {
            return new Parser(input).Parse();
        }

        private void AssertOnSuccess(ParserResult<UrlTemplate> result, Action<UrlTemplate> action)
        {
            result.Match(
                x => action(x.Result),
                x => Assert.Fail($"Parse failed. \n{string.Join("\n", x.Errors.Select(e => e.ToString()))}"));
        }
        private void AssertOnFailure(ParserResult<UrlTemplate> result, Action<IReadOnlyCollection<ParserError>> action)
        {
            result.Match(
                x => Assert.Fail("Expected to fail."),
                x => action(x.Errors));
        }
    }
}
