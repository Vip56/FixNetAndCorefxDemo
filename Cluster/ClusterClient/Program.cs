using Messages;
using Proto.Cluster;
using Proto.Cluster.Consul;
using Proto.Remote;
using System;
using System.Threading;
using ProtosReflection = Messages.ProtosReflection;

namespace ClusterClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var r = new Random();
            Serialization.RegisterFileDescriptor(ProtosReflection.Descriptor);
            Remote.Start("127.0.0.1", 120 * 100 + r.Next(100));
            Cluster.Start("MyCluster", new ConsulProvider(new ConsulProviderOptions()));
            Thread.Sleep(100);
            var pid = Cluster.GetAsync("TheName", "HelloKind").Result;
            for (var i = 0; i < 100; i++)
            {
                var res = pid.RequestAsync<HelloResponse>(new HelloRequest()).Result;
                Console.WriteLine(res.Message);
            }
            Console.ReadLine();
        }
    }
}
