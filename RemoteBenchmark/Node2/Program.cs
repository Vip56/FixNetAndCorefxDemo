using Messages;
using Proto;
using Proto.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProtosReflection = Messages.ProtosReflection;

namespace Node2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Serialization.RegisterFileDescriptor(ProtosReflection.Descriptor);
            Remote.Start("127.0.0.1", 12000);
            Actor.SpawnNamed(Actor.FromProducer(() => new EchoActor()), "remote");
            Console.ReadLine();
        }

        public class EchoActor : IActor
        {
            private PID _sender;

            public Task ReceiveAsync(IContext context)
            {
                if(context.Message is StartRemote)
                {
                    var sr = context.Message as StartRemote;
                    Console.WriteLine("Starting");
                    _sender = sr.Sender;
                    context.Respond(new Start());
                }
                else if(context.Message is Ping)
                {
                    _sender.Tell(new Pong());
                }

                return Actor.Done;
            }
        }
    }
}
