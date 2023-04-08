namespace SpaceBattle.Collections;

using System.Collections.Concurrent;
using SpaceBattle.Base;
using SpaceBattle.Base.Collections;

public class Stream<T> : IStream<T>
{
    private IProducerConsumerCollection<T> q;

    private IPullable<T> pullable;
    private IPushable<T> pushable;

    public Stream(IProducerConsumerCollection<T> collection)
    {
        this.q = collection;

        this.pullable = Container.Resolve<IPullable<T>>("Entities.Adapter.IPullable", this.q);
        this.pushable = Container.Resolve<IPushable<T>>("Entities.Adapter.IPushable", this.q);
    }

    public IPullable<T> Pullable => pullable;

    public IPushable<T> Pushable => pushable;
}
