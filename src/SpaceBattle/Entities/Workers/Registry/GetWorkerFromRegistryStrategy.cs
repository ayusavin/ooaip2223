namespace SpaceBattle.Entities.Workers.Registry;

using SpaceBattle.Base;
using SpaceBattle.Collections;

public class GetWorkerFromRegistryStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        string id = (string)argv[0];

        return Container.Resolve<IDictionary<string, IWorker>>(
            "Workers.Registry"
        )[id];
    }
}
