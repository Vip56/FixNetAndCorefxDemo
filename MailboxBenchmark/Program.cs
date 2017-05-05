using Proto;
using Proto.Mailbox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MailboxBenchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Func<IMailbox> unboundedMailbox = () => UnboundedMailbox.Create();
            RunTest(unboundedMailbox, "Unbounded mailbox");

            Console.ReadLine();
        }

        private static void RunTest(Func<IMailbox> mailbox, string name)
        {
            Stopwatch sendSw = new Stopwatch(), recvSw = new Stopwatch();
            const int n = 100 * 1000 * 1000;
            var props = Actor.FromFunc(c =>
            {
                if (c.Message is int?)
                {
                    int i = (int)c.Message;
                    if (i == n)
                    {
                        recvSw.Stop();
                        Console.WriteLine($"recv {(int)(n / recvSw.Elapsed.TotalSeconds / 1000)}K/sec ({name})");
                    }
                }
                return Actor.Done;
            });

            if (mailbox != null)
                props.WithMailbox(mailbox);

            var pid = Actor.Spawn(props);
            sendSw.Start();
            recvSw.Start();
            for(var i = 1; i <= n; i++)
            {
                pid.Tell(i);
            }
            sendSw.Stop();
            Console.WriteLine($"send {(int)(n / sendSw.Elapsed.TotalSeconds / 1000)}K/sec ({name})");
        }
    }
}
