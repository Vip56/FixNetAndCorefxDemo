using Messages;
using Proto;
using Proto.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

            var messageCount = 1000000;
            var wg = new AutoResetEvent(false);
            var props = Actor.FromProducer(() => new LocalActor(0, messageCount, wg));

            var pid = Actor.Spawn(props);
            var remote = new PID("127.0.0.1:12000", "remote");
            remote.RequestAsync<Start>(new StartRemote { Sender = pid }).Wait();

            var start = DateTime.Now;
            Console.WriteLine("Starting to send");
            var msg = new Ping();
            for(var i = 0; i< messageCount; i++)
            {
                remote.Tell(msg);
            }
            wg.WaitOne();
            var elapsed = DateTime.Now - start;
            Console.WriteLine("Elapsed {0}", elapsed);

            var t = messageCount * 2.0 / elapsed.TotalMilliseconds * 1000;
            Console.WriteLine("Throughput {0} msg / sec", t);

            Console.ReadKey();
        }
    }

    public class LocalActor : IActor
    {
        private int _count;
        private readonly int _messageCount;
        private readonly AutoResetEvent _wg;

        public LocalActor(int count, int messageCount, AutoResetEvent wg)
        {
            _count = count;
            _messageCount = messageCount;
            _wg = wg;
        }

        public Task ReceiveAsync(IContext context)
        {
            if(context.Message is Pong)
            {
                _count++;
                if(_count % 50000 == 0)
                {
                    Console.WriteLine(_count);
                }
                if(_count == _messageCount)
                {
                    _wg.Set();
                }
            }
            return Actor.Done;
        }
    }
}
