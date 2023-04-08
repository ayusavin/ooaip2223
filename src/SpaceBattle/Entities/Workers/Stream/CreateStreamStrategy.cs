namespace SpaceBattle.Entities.Workers.Stream;

using System.Collections.Concurrent;
using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;

public class CreateStreamStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        string id = (string)argv[0];

        return new CreateStreamCommand(id: id);
    }
}

class CreateStreamCommand : ICommand
{
    private string id;

    public CreateStreamCommand(string id)
    {
        this.id = id;
    }

    public void Run()
    {
        var registry = Container.Resolve<IDictionary<string, IStream<ICommand>>>("Workers.Stream.Registry");
        if (registry.ContainsKey(key: this.id))
            throw new Exception("Stream with this id is already exist");
        registry.Add(key: this.id, value: new ConcurrentQueueStream<ICommand>());
    }
}

class ConcurrentQueueStream<T> : IStream<T>
{
    IProducerConsumerCollection<T> q;
    public ConcurrentQueueStream()
    {
        q = new ConcurrentQueue<T>();
    }

    public IPullable<T> Pullable => new ConcurrentQueuePullable<T>(q: this.q);

    public IPushable<T> Pushable => new ConcurrentQueuePushable<T>(q: this.q);
}

class ConcurrentQueuePullable<T> : IPullable<T>
{

    IProducerConsumerCollection<T> q;

    public ConcurrentQueuePullable(IProducerConsumerCollection<T> q)
    {
        this.q = q;
    }

    public bool Empty()
    {
        return q.Count == 0;
    }

    public T Pull()
    {
        return q.First();
    }
}

class ConcurrentQueuePushable<T> : IPushable<T>
{

    IProducerConsumerCollection<T> q;

    public ConcurrentQueuePushable(IProducerConsumerCollection<T> q)
    {
        this.q = q;
    }

    public void Push(T value)
    {
        q.Append(value);
    }
}
