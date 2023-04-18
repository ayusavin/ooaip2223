namespace SpaceBattle.Entities.Workers.Registry;

using System.Collections.Concurrent;
using SpaceBattle.Base;
using SpaceBattle.Collections;

public class WorkerRegistryStrategy : IStrategy
{
    static private IDictionary<string, IWorker> registry;

    static WorkerRegistryStrategy()
    {
        registry = new ConcurrentDictionary<string, IWorker>();
    }
    public object Run(params object[] argv)
    {
        return Container.Resolve<ICommand>("IoC.Register", "Workers.Registry",
           (object[] _) => registry);
    }
}
