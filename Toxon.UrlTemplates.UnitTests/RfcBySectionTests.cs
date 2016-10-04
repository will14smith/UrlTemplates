using System;
using System.Collections.Generic;
using System.Linq;
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

                AssertResult(input, expected, result);
            }
        }

        [Test]
        public void Test3_2_1() { RunSection("3.2.1 Variable Expansion"); }
        [Test]
        public void Test3_2_2() { RunSection("3.2.2 Simple String Expansion"); }
        [Test]
        public void Test3_2_3() { RunSection("3.2.3 Reserved Expansion"); }
        [Test]
        public void Test3_2_4() { RunSection("3.2.4 Fragment Expansion"); }
        [Test]
        public void Test3_2_5() { RunSection("3.2.5 Label Expansion with Dot-Prefix"); }
        [Test]
        public void Test3_2_6() { RunSection("3.2.6 Path Segment Expansion"); }
        [Test]
        public void Test3_2_7() { RunSection("3.2.7 Path-Style Parameter Expansion"); }
        [Test]
        public void Test3_2_8() { RunSection("3.2.8 Form-Style Query Expansion"); }
        [Test]
        public void Test3_2_9() { RunSection("3.2.9 Form-Style Query Continuation"); }

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

            return GetValue(value);
        }

        private IValue GetValue(JToken token)
        {
            if (token == null)
            {
                return new NullValue();
            }

            if (token is JValue)
            {
                var val = ((JValue)token).Value;
                if (val == null)
                {
                    return new NullValue();
                }

                return new ConstantStringValue(val.ToString());
            }

            if (token is JArray)
            {
                var items = token.Select(GetValue).Cast<IStringValue>();

                return new ConstantArrayValue(items);
            }

            if (token is JObject)
            {
                var obj = (JObject)token;
                var items = new Dictionary<string, IStringValue>();

                foreach (var item in obj.Properties())
                {
                    items.Add(item.Name, (IStringValue)GetValue(item.Value));
                }

                return new ConstantDictionaryValue(items);
            }

            throw new NotImplementedException();
        }
    }
}
