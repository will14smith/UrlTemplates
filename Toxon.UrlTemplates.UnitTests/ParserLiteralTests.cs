using System.Linq;
using NUnit.Framework;

namespace Toxon.UrlTemplates.UnitTests
{
    public class ParserLiteralTests
    {
        [Test]
        public void Standard()
        {
            var input = "hello-world_123.foo";
            var parser = new Parser(input);

            parser.Parse().Match(
                x =>
                {
                    var output = x.Result.ResolveToString(null, false);

                    Assert.AreEqual(input, output);
                },
                x =>
                {
                    Assert.Fail($"Parse failed. \n{string.Join("\n", x.Errors.Select(e => e.ToString()))}");
                });
        }

        [Test]
        public void PercentEncoded()
        {
            var input = "%20";
            var parser = new Parser(input);

            parser.Parse().Match(
                x =>
                {
                    var output = x.Result.ResolveToString(null, false);

                    Assert.AreEqual(input, output);
                },
                x =>
                {
                    Assert.Fail($"Parse failed. \n{string.Join("\n", x.Errors.Select(e => e.ToString()))}");
                });
        }

        [Test]
        public void PercentTruncated1()
        {
            var input = "%";
            var parser = new Parser(input);

            parser.Parse().Match(
                x => Assert.Fail("Expected to fail."),
                x =>
                {
                    // TODO check error
                });
        }
        [Test]
        public void PercentTruncated2()
        {
            var input = "%2";
            var parser = new Parser(input);

            parser.Parse().Match(
                x => Assert.Fail("Expected to fail."),
                x =>
                {
                    // TODO check error
                });
        }
        [Test]
        public void PercentInvalid1()
        {
            var input = "%G1";
            var parser = new Parser(input);

            parser.Parse().Match(
                x => Assert.Fail("Expected to fail."),
                x =>
                {
                    // TODO check error
                });
        }
        [Test]
        public void PercentInvalid2()
        {
            var input = "%1G";
            var parser = new Parser(input);

            parser.Parse().Match(
                x => Assert.Fail("Expected to fail."),
                x =>
                {
                    // TODO check error
                });
        }
    }
}
