using NUnit.Framework;

namespace Toxon.UrlTemplates.UnitTests.Parser
{
    public partial class ParserTests
    {
        [Test]
        public void Level3Variable()
        {
            var input = "{.foo123_%20}";
            var result = Parse(input);

            AssertOnSuccess(result, x =>
            {
                var output = x.ToString();

                Assert.AreEqual(input, output);
            });
        }
        [Test]
        public void Level3MultiVariable()
        {
            var input = "{.foo123_%20,bar456_%21}";
            var result = Parse(input);

            AssertOnSuccess(result, x =>
            {
                var output = x.ToString();

                Assert.AreEqual(input, output);
            });
        }
    }
}
