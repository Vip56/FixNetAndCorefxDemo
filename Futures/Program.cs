using Proto;
using System;

namespace Futures
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var props = Actor.FromFunc(ctx =>
            {
                if(ctx.Message is string)
                {
                    ctx.Respond("hey");
                }
                return Actor.Done;
            });

            var pid = Actor.Spawn(props);
            var reply = pid.RequestAsync<object>("hello").Result;

            Console.WriteLine(reply);

            Console.ReadKey();
        }
    }
}
