using System.Linq;
using NUnit.Framework;

namespace Toxon.UrlTemplates.UnitTests
{
    public partial class ParserTests
    {
        [Test]
        public void Level2Variable()
        {
            var input = "{+foo123_%20}";
            var result = Parse(input);

            AssertOnSuccess(result, x =>
            {
                var output = x.ToString();

                Assert.AreEqual(input, output);
            });
        }
    }
}
