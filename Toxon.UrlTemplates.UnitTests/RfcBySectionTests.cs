using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Toxon.UrlTemplates.Values;

namespace Toxon.UrlTemplates.UnitTests
{
    public class RfcBySectionTests
    {
        public void RunSection(string section)
        {
            var data = SampleData.Load(TestContext.CurrentContext.TestDirectory + "/uritemplate-test/spec-examples-by-section.json");

            var test = data.Data[section];

            foreach (var testcase in test.TestCases)
            {
                var input = (string)((JValue)testcase.First).Value;
                var expected = testcase.Last;

                var template = UrlTemplate.Parse(input);
                var variables = new JsonValueResolver(test.Variables);

                var result = template.ResolveToString(variables, false);

                AssertResult(expected, result);
            }
        }

        [Test]
        public void Test3_2_2()
        {
            RunSection("3.2.2 Simple String Expansion");
        }

        private void AssertResult(JToken expected, string result)
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

            CollectionAssert.Contains(expectedValues, result);
        }
    }

    public class JsonValueResolver : IValueResolver
    {
        private readonly JObject _variables;

        public JsonValueResolver(JObject variables)
        {
            _variables = variables;
        }

        public IValue GetValue(string key)
        {
            var value = _variables[key];

            if (value == null)
            {
                return new NullValue();
            }

            if (value is JValue)
            {
                var val = ((JValue)value).Value;
                if (val == null)
                {
                    return new NullValue();
                }

                return new ConstantStringValue(val.ToString());
            }

            throw new NotImplementedException();
        }
    }
}
