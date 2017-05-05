using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtoActorConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var props = Actor.FromProducer(() => new HelloActor());
            var pid = Actor.Spawn(props);

            pid.Tell(new Hello
            {
                Who = "PortoActor"
            });

            Console.ReadKey();
        }

        internal class Hello
        {
            public string Who { get; set; }
        }

        internal class HelloActor : IActor
        {
            public Task ReceiveAsync(IContext context)
            {
                var msg = context.Message;
                if(msg is Hello)
                {
                    var r = msg as Hello;
                    Console.WriteLine($"Hello {r.Who}");
                }
                return Actor.Done;
            }
        }
    }
}
