using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Toxon.UrlTemplates.UnitTests.Execution
{
    public class PartialExpansionTests
    {
        private const string testNonExplodeUrl = "http://host{.a,b}{/c,d}{?e,f}{&g,h}{#i,j}";

        [Test]
        [TestCase("{\"a\": \"a\"}", "http://host.a{.b}{/c,d}{?e,f,g,h}{#i,j}")]
        [TestCase("{\"b\": \"b\"}", "http://host{.a}.b{/c,d}{?e,f,g,h}{#i,j}")]
        [TestCase("{\"c\": \"c\"}", "http://host{.a,b}/c{/d}{?e,f,g,h}{#i,j}")]
        [TestCase("{\"d\": \"d\"}", "http://host{.a,b}{/c}/d{?e,f,g,h}{#i,j}")]
        [TestCase("{\"e\": \"e\"}", "http://host{.a,b}{/c,d}?e=e{&f,g,h}{#i,j}")]
        [TestCase("{\"f\": \"f\"}", "http://host{.a,b}{/c,d}{?e}&f=f{&g,h}{#i,j}")]
        [TestCase("{\"g\": \"g\"}", "http://host{.a,b}{/c,d}{?e,f}&g=g{&h}{#i,j}")]
        [TestCase("{\"h\": \"h\"}", "http://host{.a,b}{/c,d}{?e,f,g}&h=h{#i,j}")]
        [TestCase("{\"i\": \"i\"}", "http://host{.a,b}{/c,d}{?e,f,g,h}#i{,j}")]
        [TestCase("{\"j\": \"j\"}", "http://host{.a,b}{/c,d}{?e,f,g,h}{#i},j")]
        public void TestNonExplode(string json, string output)
        {
            Test(testNonExplodeUrl, json, output);
        }


        private void Test(string input, string variablesJson, string expected)
        {
            var variables = new JsonValueResolver(JObject.Parse(variablesJson));
            var template = UrlTemplate.Parse(input);

            var output = template.ResolveToString(variables, true);

            Assert.AreEqual(expected, output);
        }
    }
}
