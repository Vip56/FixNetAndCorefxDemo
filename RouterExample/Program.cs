using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proto.Router;
using Proto;

namespace RouterExample
{
    internal class Message : IHashable
    {
        public string Text;

        public string HashBy()
        {
            return Text;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    internal class MyActor : IActor
    {
        public Task ReceiveAsync(IContext context)
        {
            var msg = context.Message as Message;
            if(msg != null)
            {
                Console.WriteLine($"Actor {context.Self.Id} got message '{msg.Text}'");
            }
            return Actor.Done;
        }
    }

    /// <summary>
    /// 演示各种路由算法的效果
    /// </summary>
    public class Program
    {
        private static readonly Props MyActorProps = Actor.FromProducer(() => new MyActor());

        public static void Main(string[] args)
        {
            //TestBroadcastPool();
            //TestBroadcastGroup();

            //TestRandomPool();
            //TestRandomGroup();

            //TestRoundRobinPool();
            //TestRoundRobinGroup();

            //TestConsistentHashPool();
            //TestConsistentHashGroup();

            Console.ReadKey();
        }

        /// <summary>
        /// 路由中的分组，按照routees参数传递多少个就会存在多少分组，其context.Self.Id的会显示为$1~$?
        /// </summary>
        private static void TestBroadcastGroup()
        {
            var props = Router.NewBroadcastGroup(
                MyActorProps,
                Actor.Spawn(MyActorProps),
                Actor.Spawn(MyActorProps),
                Actor.Spawn(MyActorProps),
                Actor.Spawn(MyActorProps));
            for(var i = 0;i < 10;i++)
            {
                var pid = Actor.Spawn(props);
                pid.Tell(new Message { Text = $"{i % 4}" });
            }
        }

        /// <summary>
        /// 在分组的前提下将会划分队列，其context.Self.Id参数会显示为$2/router/$1
        /// </summary>
        private static void TestBroadcastPool()
        {
            var props = Router.NewBroadcastPool(MyActorProps, 2);
            var pid = Actor.Spawn(props);
            for(var i = 0; i< 10;i++)
            {
                pid.Tell(new Message{ Text = $"{i % 4}" });
            }
        }

        /// <summary>
        /// 将会启动若干分组使用一致性哈希进行均衡处理
        /// </summary>
        private static void TestConsistentHashGroup()
        {
            var props = Router.NewConsistentHashGroup(
                MyActorProps,
                Actor.Spawn(MyActorProps),
                Actor.Spawn(MyActorProps),
                Actor.Spawn(MyActorProps),
                Actor.Spawn(MyActorProps));
            var pid = Actor.Spawn(props);
            for (var i = 0; i < 10; i++)
            {
                pid.Tell(new Message { Text = $"{i % 4}" });
            }
        }

        /// <summary>
        /// 将会启动若干队列使用一致性哈希进行均衡处理
        /// </summary>
        private static void TestConsistentHashPool()
        {
            var props = Router.NewConsistentHashPool(MyActorProps, 5);
            var pid = Actor.Spawn(props);
            for(var i =0;i < 10;i++)
            {
                pid.Tell(new Message { Text = $"{i % 4}" });
            }
        }

        /// <summary>
        /// 将会启动若干队列进行轮转均衡处理
        /// </summary>
        private static void TestRoundRobinGroup()
        {
            var props = Router.NewRoundRobinGroup(
                MyActorProps,
                Actor.Spawn(MyActorProps),
                Actor.Spawn(MyActorProps),
                Actor.Spawn(MyActorProps),
                Actor.Spawn(MyActorProps));
            var pid = Actor.Spawn(props);
            for(var i = 0;i < 10; i++)
            {
                pid.Tell(new Message { Text = $"{i % 4}" });
            }
        }

        /// <summary>
        /// 将会启动若干队列进行轮转均衡处理
        /// </summary>
        private static void TestRoundRobinPool()
        {
            var props = Router.NewRoundRobinPool(MyActorProps, 5);
            var pid = Actor.Spawn(props);
            for(var i = 0; i < 10; i++)
            {
                pid.Tell(new Message { Text = $"{i % 4}" });
            }
        }

        /// <summary>
        /// 将会启动若干分组进行随机均衡处理
        /// </summary>
        private static void TestRandomGroup()
        {
            var props = Router.NewRandomGroup(
                MyActorProps,
                Actor.Spawn(MyActorProps),
                Actor.Spawn(MyActorProps),
                Actor.Spawn(MyActorProps),
                Actor.Spawn(MyActorProps));
            var pid = Actor.Spawn(props);
            for(var i = 0;i < 10;i ++)
            {
                pid.Tell(new Message { Text = $"{i % 4}" });
            }
        }

        /// <summary>
        /// 将会启动若干队列进行随机均衡处理
        /// </summary>
        private static void TestRandomPool()
        {
            var props = Router.NewRandomPool(MyActorProps, 5);
            var pid = Actor.Spawn(props);
            for (var i = 0; i < 10; i++)
            {
                pid.Tell(new Message { Text = $"{i % 4}" });
            }
        }
    }
}
