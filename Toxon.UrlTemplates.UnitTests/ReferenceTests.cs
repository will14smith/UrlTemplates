using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Toxon.UrlTemplates.UnitTests
{
    public abstract class ReferenceTests
    {
        internal void RunContext(SampleContext context)
        {
            foreach (var testcase in context.TestCases)
            {
                var input = (string)((JValue)testcase.First).Value;
                var expected = testcase.Last;

                var template = UrlTemplate.Parse(input);
                var variables = new JsonValueResolver(context.Variables);

                var result = template.ResolveToString(variables, false);

                AssertResult(input, expected, result);
            }
        }

        private void AssertResult(string template, JToken expected, string result)
        {
            List<string> expectedValues;

            if (expected is JValue)
            {
                expectedValues = new List<string> { (string)((JValue)expected).Value };
            }
            else if (expected is JArray)
            {
                expectedValues = expected.Select(value => (string)((JValue)value).Value).ToList();
            }
            else
            {
                throw new NotImplementedException();
            }

            Console.WriteLine("Template: {0}", template);
            CollectionAssert.Contains(expectedValues, result);
        }

    }
}