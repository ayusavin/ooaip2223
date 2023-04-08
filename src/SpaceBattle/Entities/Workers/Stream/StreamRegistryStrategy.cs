namespace SpaceBattle.Entities.Workers.Stream;

using System.Collections.Concurrent;
using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;

public class StreamRegistryStrategy : IStrategy
{
    static private IDictionary<string, IStream<ICommand>> registry;

    static StreamRegistryStrategy()
    {
        registry = new ConcurrentDictionary<string, IStream<ICommand>>();
    }

    public object Run(params object[] argv)
    {
        return Container.Resolve<ICommand>("IoC.Register", "Workers.Stream.Registry",
            (object[] _) => registry);
    }
}
