using System.Linq;
using NUnit.Framework;

namespace Toxon.UrlTemplates.UnitTests
{
    public partial class ParserTests
    {
        [Test]
        public void Level4Explode()
        {
            var input = "{?foo123_%20*}";
            var result = Parse(input);

            AssertOnSuccess(result, x =>
            {
                var output = x.ToString();

                Assert.AreEqual(input, output);
            });
        }
        [Test]
        public void Level4Prefix()
        {
            var input = "{.foo123_%20,bar456_%21:5}";
            var result = Parse(input);

            AssertOnSuccess(result, x =>
            {
                var output = x.ToString();

                Assert.AreEqual(input, output);
            });
        }
        [Test]
        public void Level4InvalidPrefix0()
        {
            var input = "{.foo123_%20,bar456_%21:0}";
            var result = Parse(input);

            AssertOnFailure(result, x =>
            {
                // TODO check error
            });
        }
        [Test]
        public void Level4InvalidPrefix10000()
        {
            var input = "{.foo123_%20,bar456_%21:10000}";
            var result = Parse(input);

            AssertOnFailure(result, x =>
            {
                // TODO check error
            });
        }
    }
}
