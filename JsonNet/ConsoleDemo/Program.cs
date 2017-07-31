using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;

namespace ConsoleDemo
{
    public class Program
    {
        public class DotoA
        {
            public DateTime Now { get; set; }

            public Guid Id { get; set; }

            public string Name { get; set; }

            public int Age { get; set; }

            public List<int> Ages { get; set; }

            public List<string> Names { get; set; }
        }

        public static void Main(string[] args)
        {
            var _serializer = new JsonSerializer
            {
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                Formatting = Formatting.None,
                CheckAdditionalContent = true,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                NullValueHandling = NullValueHandling.Ignore
            };

            DotoA a = new DotoA
            {
                Now = DateTime.Now,
                Id = Guid.NewGuid(),
                Name = "test1",
                Age = 11,
                Ages = new List<int>
                {
                    1,2,3,4,5,6,7,7,8,9,0
                },
                Names = new List<string>
                {
                    "1","2","3","4","5"
                }
            };

            string msgStr;
            using (var sw = new StringWriter())
            {
                _serializer.Serialize(sw, a);
                msgStr = sw.GetStringBuilder().ToString();
            }

            Console.WriteLine(msgStr);

            using (var jr = new JsonTextReader(new StringReader(msgStr)))
            {
                var obj = _serializer.Deserialize<DotoA>(jr);
                Console.WriteLine(JsonConvert.SerializeObject(obj));
            }

            Console.ReadKey();
        }
    }
}
