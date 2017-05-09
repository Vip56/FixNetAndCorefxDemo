using Messages;
using Proto.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProtosReflection = Messages.ProtosReflection;

namespace Node1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Serialization.RegisterFileDescriptor(ProtosReflection.Descriptor);
            Remote.Start("127.0.0.1", 12001);
            var pid = Remote.SpawnNamedAsync("127.0.0.1:12000", "remote", "hello", TimeSpan.FromSeconds(5)).Result;
            var res = pid.RequestAsync<HelloResponse>(new HelloRequest()).Result;
            Console.WriteLine(res.Message);
            Console.ReadKey();
        }
    }
}
