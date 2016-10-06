using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Toxon.UrlTemplates.UnitTests.Execution
{
    public class ExecutorTests
    {
        [Test]
        public void Test()
        {
            var input = "/lookup{?Stra%C3%9Fe}";
            var expected = "/lookup?Stra%C3%9Fe=Gr%C3%BCner%20Weg";

            var variables = new JsonValueResolver(JObject.Parse("{\"Stra%C3%9Fe\": \"Grüner Weg\"}"));
            var template = UrlTemplate.Parse(input);

            var output = template.ResolveToString(variables, false);

            Assert.AreEqual(expected, output);
        }
    }
}
