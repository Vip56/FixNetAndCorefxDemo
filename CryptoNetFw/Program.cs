using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoNetFw
{
    class Program
    {
        static void Main(string[] args)
        {
            DESTest test = new DESTest();

            string encodeStr = test.Encode("我是一个测试的数据");

            string decodeStr = test.Decode("802B73F78D1197EF48AF5DF937C885B88E1A24C7BA016CB44DD871193DA7F02C");
        }
    }
}
