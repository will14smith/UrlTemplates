using System.Linq;
using NUnit.Framework;

namespace Toxon.UrlTemplates.UnitTests
{
    public partial class ParserTests
    {
        [Test]
        public void Level1Variable()
        {
            var input = "{foo123_%20}";
            var result = Parse(input);

            AssertOnSuccess(result, x =>
            {
                var output = x.ToString();

                Assert.AreEqual(input, output);
            });
        }
        [Test]
        public void Level1LiteralVariableLiteral()
        {
            var input = "hello{foo}world";
            var result = Parse(input);

            AssertOnSuccess(result, x =>
            {
                var output = x.ToString();

                Assert.AreEqual(input, output);
            });
        }
    }
}
