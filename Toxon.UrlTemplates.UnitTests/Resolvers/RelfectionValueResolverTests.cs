using NUnit.Framework;
using Toxon.UrlTemplates.Resolvers;
using Toxon.UrlTemplates.Values;

namespace Toxon.UrlTemplates.UnitTests.Resolvers
{
    public class RelfectionValueResolverTests
    {
        [Test]
        public void TestStringField()
        {
            var resolver = new ReflectionValueResolver(new
            {
                a = "abc"
            });

            var value = resolver.GetValue("a");

            Assert.IsInstanceOf<IStringValue>(value);
            Assert.AreEqual("abc", ((IStringValue)value).Value);
        }
    }
}
