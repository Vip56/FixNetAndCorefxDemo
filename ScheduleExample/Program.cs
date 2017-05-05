using Proto;
using Proto.Schedulers.SimpleScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduleExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var props = Actor.FromProducer(() => new ScheduleActor());
            var pid = Actor.Spawn(props);

            Console.ReadLine();
        }
    }

    public class Hello { }

    public class HickUp { }

    public class AbortHickUp { }

    public class Greet
    {
        public string Who { get; }

        public Greet(string who)
        {
            Who = who;
        }
    }

    public class SimpleMessage
    {
        public string Msg { get; }

        public SimpleMessage(string msg)
        {
            Msg = msg;
        }
    }

    /// <summary>
    /// 对各种编排方式进行演示，比如定时发送指定的消息到指定的地方后定时发送消息以及取消。
    /// </summary>
    public class ScheduleActor : IActor
    {
        private ISimpleScheduler scheduler = new SimpleScheduler();

        private CancellationTokenSource timer;

        private int counter = 0;

        public Task ReceiveAsync(IContext context)
        {
            if(context.Message is Started)
            {
                var pid = context.Spawn(Actor.FromProducer(() => new ScheduleGreetActor()));
                scheduler.ScheduleTellOnce(TimeSpan.FromMilliseconds(100), context.Self, new SimpleMessage("test 1"))
                        .ScheduleTellOnce(TimeSpan.FromMilliseconds(200), context.Self, new SimpleMessage("test 2"))
                        .ScheduleTellOnce(TimeSpan.FromMilliseconds(300), context.Self, new SimpleMessage("test 3"))
                        .ScheduleTellOnce(TimeSpan.FromMilliseconds(400), context.Self, new SimpleMessage("test 4"))
                        .ScheduleTellOnce(TimeSpan.FromMilliseconds(500), context.Self, new SimpleMessage("test 5"))
                        .ScheduleRequestOnce(TimeSpan.FromSeconds(1), context.Self, pid, new Greet("Daniel"))
                        .ScheduleTellOnce(TimeSpan.FromSeconds(5), context.Self, new Hello());
            }
            else if(context.Message is Hello)
            {
                var hl = context.Message as Hello;
                Console.WriteLine($"Hello Once,let's give you a hick");
                scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(3), TimeSpan.FromMilliseconds(500), context.Self, new HickUp(), out timer);
            }
            else if(context.Message is HickUp)
            {
                var hu = context.Message as HickUp;
                counter++;
                Console.WriteLine("Hello!");

                if(counter == 5)
                {
                    timer.Cancel();
                    context.Self.Tell(new AbortHickUp());
                }
            }
            else if(context.Message is AbortHickUp)
            {
                Console.WriteLine($"Aborted hickup after {counter} times");
                Console.WriteLine($"All this was scheduled calls, have fun!");
            }
            else if(context.Message is Greet)
            {
                var msg = context.Message as Greet;
                Console.WriteLine($"Thanks {msg.Who}");
            }
            else if(context.Message is SimpleMessage)
            {
                var sm = context.Message as SimpleMessage;
                Console.WriteLine(sm.Msg);
            }
            return Actor.Done;
        }
    }

    public class ScheduleGreetActor : IActor
    {
        public Task ReceiveAsync(IContext context)
        {
            if(context.Message is Greet)
            {
                var msg = context.Message as Greet;
                Console.WriteLine($"Hi {msg.Who}");
                context.Sender.Tell(new Greet("Roger"));
            }
            return Actor.Done;
        }
    }
}
