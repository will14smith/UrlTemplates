using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Toxon.UrlTemplates.UnitTests
{
    class SampleContext
    {
        public int Level { get; set; }
        public JObject Variables { get; set; }
        public JArray TestCases { get; set; }
    }

    class SampleData
    {
        public Dictionary<string, SampleContext> Data { get; set; }

        public static SampleData Load(string fileName)
        {
            var file = File.ReadAllText(fileName);

            var data = JsonConvert.DeserializeObject<Dictionary<string, SampleContext>>(file);

            return new SampleData { Data = data };
        }
    }
}
