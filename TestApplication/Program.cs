using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestApplication
{
    class Program
    {

        public class SimpleModeRules
        {
            public int someParam { get; private set; }
            public int TestVisualization;
        }

        static void Main(string[] args)
        {
            SimpleModeRules test = new SimpleModeRules();
            test.TestVisualization = 555;
            string testString = "{\"someParam\" : 100500}";
            var v = JsonFx.Json.JsonReader.Deserialize<SimpleModeRules>(testString);
            test = JsonFx.Json.JsonReader.Deserialize<SimpleModeRules>(testString, test);
            Console.WriteLine(test.TestVisualization);
            Console.WriteLine(test.someParam);
            Console.ReadKey();
        }
    }
}