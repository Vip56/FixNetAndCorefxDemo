using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarsDecode
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                if (Directory.Exists(args[0]))
                {
                    var filelist = Directory.GetFiles(args[0], "*.xlog");

                }
            }
            else if(args.Length == 2)
            {

            }
            else
            {

            }
        }

        static void ParseFile(string file, string outfile)
        {

        }
    }
}
