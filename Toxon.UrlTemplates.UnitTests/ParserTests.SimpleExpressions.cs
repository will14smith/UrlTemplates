using System.Linq;
using NUnit.Framework;

namespace Toxon.UrlTemplates.UnitTests
{
    public partial class ParserTests
    {
        [Test]
        public void PlainSingleVariable()
        {
            var input = "{foo}";
            var result = Parse(input);

            AssertOnSuccess(result, x =>
            {
                var output = x.ToString();

                Assert.AreEqual(input, output);
            });
        }

        [Test]
        public void PlainMultiVariable()
        {
            var input = "{foo,bar}";
            var result = Parse(input);

            AssertOnSuccess(result, x =>
            {
                var output = x.ToString();

                Assert.AreEqual(input, output);
            });
        }
    }
}
