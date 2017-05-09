using Messages;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Consul;
using Proto.Remote;
using System;
using ProtosReflection = Messages.ProtosReflection;

namespace ClusterServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var r = new Random();
            Serialization.RegisterFileDescriptor(ProtosReflection.Descriptor);
            var props = Actor.FromFunc(ctx =>
            {
                if (ctx.Message is HelloRequest)
                {
                    ctx.Respond(new HelloResponse
                    {
                        Message = "Hello from node 2"
                    });
                    Console.WriteLine("Receive Msg");
                }
                return Actor.Done;
            });

            Remote.RegisterKnownKind("HelloKind", props);
            Remote.Start("127.0.0.1", 120 * 100 + r.Next(100));
            Cluster.Start("MyCluster", new ConsulProvider(new ConsulProviderOptions()));

            Console.ReadLine();
        }
    }
}
