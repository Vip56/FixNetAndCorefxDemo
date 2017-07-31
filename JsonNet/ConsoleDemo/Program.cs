using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

            Console.WriteLine(JsonConvert.SerializeObject(a));

            Console.ReadKey();
        }
    }
}
