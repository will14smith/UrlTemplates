using NUnit.Framework;

namespace Toxon.UrlTemplates.UnitTests.Parser
{
    public partial class ParserTests
    {
        //TODO check pct-encoding of special chars

        [Test]
        public void SimpleLiteral()
        {
            var input = "hello-world_123.foo";
            var result = Parse(input);

            AssertOnSuccess(result, x =>
            {
                var output = x.ToString();

                Assert.AreEqual(input, output);
            });
        }

        [Test]
        public void PercentEncoded()
        {
            var input = "%20";
            var result = Parse(input);

            AssertOnSuccess(result, x =>
            {
                var output = x.ToString();

                Assert.AreEqual(input, output);
            });
        }

        [Test]
        public void PercentTruncated1()
        {
            var input = "%";
            var result = Parse(input);

            AssertOnFailure(result, x =>
            {
                //TODO check error
            });
        }
        [Test]
        public void PercentTruncated2()
        {
            var input = "%2";
            var result = Parse(input);

            AssertOnFailure(result, x =>
            {
                //TODO check error
            });
        }
        [Test]
        public void PercentInvalid1()
        {
            var input = "%G1";
            var result = Parse(input);

            AssertOnFailure(result, x =>
            {
                //TODO check error
            });
        }
        [Test]
        public void PercentInvalid2()
        {
            var input = "%1G";
            var result = Parse(input);

            AssertOnFailure(result, x =>
            {
                //TODO check error
            });
        }

        [Test]
        public void EncodingSpecialChars()
        {
            var input = "€";
            var expected = "%E2%82%AC";

            var result = Parse(input);

            AssertOnSuccess(result, x =>
            {
                var output = x.ToString();

                Assert.AreEqual(expected, output);
            });
        }
    }
}
