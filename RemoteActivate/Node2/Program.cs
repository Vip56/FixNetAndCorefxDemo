using Messages;
using Proto;
using Proto.Remote;
using System;
using ProtosReflection = Messages.ProtosReflection;

namespace Node2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Serialization.RegisterFileDescriptor(ProtosReflection.Descriptor);
            var props = Actor.FromFunc(ctx =>
            {
                if(ctx.Message is HelloRequest)
                {
                    var msg = ctx.Message as HelloRequest;
                    ctx.Respond(new HelloResponse
                    {
                        Message = "Hello from node 2",
                    });
                }
                return Actor.Done;
            });

            Remote.RegisterKnownKind("hello", props);
            Remote.Start("127.0.0.1", 12000);

            Console.ReadKey();
        }
    }
}
