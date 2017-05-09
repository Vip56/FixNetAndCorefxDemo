using Microsoft.Extensions.Logging;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Supervisor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Proto.Log.SetLoggerFactory(new LoggerFactory().AddConsole(minLevel: LogLevel.Debug));

            var props = Actor.FromProducer(() => new ParentActor()).WithSupervisor(new OneForOneStrategy(Decider.Decide, 1, null));

            var actor = Actor.Spawn(props);
            actor.Tell(new Hello
            {
                Who = "Ales"
            });

            Thread.Sleep(TimeSpan.FromSeconds(1));
            actor.Tell(new Recoverable());
            actor.Tell(new Fatal());

            actor.Stop();
            Console.ReadLine();
        }

        internal class Decider
        {
            public static SupervisorDirective Decide(PID pid, Exception reason)
            {
                if (reason is RecoverableException)
                {
                    return SupervisorDirective.Restart;
                }
                else if (reason is FatalException)
                {
                    return SupervisorDirective.Stop;
                }
                else
                    return SupervisorDirective.Escalate;
            }
        }

        internal class ParentActor : IActor
        {
            public Task ReceiveAsync(IContext context)
            {
                PID child;
                if(context.Children == null || context.Children.Count == 0)
                {
                    var props = Actor.FromProducer(() => new ChildActor());
                    child = context.Spawn(props);
                }
                else
                {
                    child = context.Children.First();
                }

                if(context.Message is Hello)
                {
                    child.Tell(context.Message);
                }
                else if(context.Message is Recoverable)
                {
                    child.Tell(context.Message);
                }
                else if(context.Message is Fatal)
                {
                    child.Tell(context.Message);
                }
                else if(context.Message is Terminated)
                {
                    var r = context.Message as Terminated;
                    Console.WriteLine($"Watched actor was Terminated, {r.Who}");
                }
                return Actor.Done;
            }
        }

        internal class ChildActor : IActor
        {
            private ILogger logger = Log.CreateLogger<ChildActor>();

            public Task ReceiveAsync(IContext context)
            {
                var msg = context.Message;

                if(msg is Hello)
                {
                    var r = msg as Hello;
                    logger.LogDebug($"Hello {r.Who}");
                }
                else if(msg is Recoverable)
                {
                    throw new RecoverableException();
                }
                else if(msg is Fatal)
                {
                    throw new FatalException();
                }
                else if(msg is Started)
                {
                    logger.LogDebug("Started, initialize actor here");
                }
                else if(msg is Stopping)
                {
                    logger.LogDebug("Stopped, actor and it's children are stopped");
                }
                else if(msg is Restarting)
                {
                    logger.LogDebug("Restarting, actor is about restart");
                }
                return Actor.Done;
            }
        }

        internal class Hello
        {
            public string Who;
        }

        internal class RecoverableException : Exception { }

        internal class FatalException : Exception { }

        internal class Fatal { }

        internal class Recoverable { }
    }
}
