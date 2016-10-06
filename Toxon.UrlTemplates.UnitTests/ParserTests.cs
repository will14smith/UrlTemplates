using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Toxon.UrlTemplates.Parsing;

namespace Toxon.UrlTemplates.UnitTests
{
    public partial class ParserTests
    {
        [Test]
        public void CanParseAllSamples()
        {
            var data = SampleData.Load(TestContext.CurrentContext.TestDirectory + "/uritemplate-test/spec-examples.json");
            foreach (var ctx in data.Data)
                foreach (var testCase in ctx.Value.TestCases)
                    AssertOnSuccess(Parse(testCase.First.ToString()), x => { });

            data = SampleData.Load(TestContext.CurrentContext.TestDirectory + "/uritemplate-test/extended-tests.json");
            foreach (var ctx in data.Data)
                foreach (var testCase in ctx.Value.TestCases)
                    AssertOnSuccess(Parse(testCase.First.ToString()), x => { });

            //TODO add negative-tests.json
        }

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
