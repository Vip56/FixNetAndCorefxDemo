using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCorefx
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DESTest test = new DESTest();
            var encodeStr = test.Encode("我是一个测试的数据");
            var decodeStr = test.Decode(encodeStr);

            ImageTest imageTest = new ImageTest();
            imageTest.Process().Wait();

            Console.ReadKey();
        }
    }
}
