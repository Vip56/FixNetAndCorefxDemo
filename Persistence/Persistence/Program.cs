using Messages;
using Proto;
using Proto.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proto.Persistence.Sqlite;
using Event = Proto.Persistence.Event;
using Snapshot = Proto.Persistence.Snapshot;

public class Program
{
    public static void Main(string[] args)
    {

    }
}

public class MyPersistenceActor : IActor
{
    private PID _loopActor;
    private State _state = new State();
    private readonly Persistence _persistence;

    public MyPersistenceActor(IProvider provider)
    {

    }

    public Task ReceiveAsync(IContext context)
    {
        throw new NotImplementedException();
    }
}

