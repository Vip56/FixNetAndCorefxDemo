using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


public class Program
{
    /// <summary>
    /// 表示接收方设置一个接收超时时间，如果在规定时间内没有收到任何一个消息则会发送ReceiveTimeout类型的消息，
    /// 当然如果发送的消息实现了接口INotInfluenceReceiveTimeout则这个消息是不会影响ReceiveTimeout的发送即demo中
    /// 开始发送的消息因为间隔没有超过1s所以不会发送这个消息，而NoInfluence消息因为实现了该接口所以依然会接收到
    /// ReceiveTimeout消息
    /// </summary>
    public static void Main(string[] args)
    {
        var c = 0;
        var props = Actor.FromFunc(context =>
        {
            if (context.Message is Started)
            {
                Console.WriteLine($"{DateTime.Now} Started");
                context.SetReceiveTimeout(TimeSpan.FromSeconds(1));
            }
            else if (context.Message is ReceiveTimeout)
            {
                c++;
                Console.WriteLine($"{DateTime.Now} ReceiveTimeout:{c}");
            }
            else if (context.Message is NoInfluence)
            {
                Console.WriteLine($"{DateTime.Now} Received a no-influence message");
            }
            else if (context.Message is string)
            {
                var s = context.Message as string;
                Console.WriteLine($"{DateTime.Now} Received message:{s}");
            }
            return Actor.Done;
        });

        var pid = Actor.Spawn(props);
        for(var i = 0;i < 20; i++)
        {
            pid.Tell("hello");
            Thread.Sleep(500);
        }

        for (var i =0;i < 20; i++)
        {
            pid.Tell(new NoInfluence());
            Thread.Sleep(500);
        }

        pid.Tell("cancel");

        Console.WriteLine("Hit [return] to finish");
        Console.ReadLine();
    }
}

internal class NoInfluence : INotInfluenceReceiveTimeout
{

}
