using NUnit.Framework;

namespace Toxon.UrlTemplates.UnitTests
{
    public class ExtendedTests : ReferenceTests
    {
        public void RunSection(string section)
        {
            var data = SampleData.Load(TestContext.CurrentContext.TestDirectory + "/uritemplate-test/extended-tests.json");

            RunContext(data.Data[section]);
        }

        [Test, Ignore("TODO")]
        public void TestAdditional1() { RunSection("Additional Examples 1"); }
        [Test]
        public void TestAdditional2() { RunSection("Additional Examples 2"); }
        [Test, Ignore("Disagree with tests")]
        public void TestAdditional3() { RunSection("Additional Examples 3: Empty Variables"); }
        [Test]
        public void TestAdditional4() { RunSection("Additional Examples 4: Numeric Keys"); }

    }
}
