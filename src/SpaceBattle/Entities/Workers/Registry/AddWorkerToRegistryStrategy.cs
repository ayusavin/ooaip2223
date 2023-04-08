namespace SpaceBattle.Entities.Workers.Registry;

using SpaceBattle.Base;
using SpaceBattle.Collections;

public class AddWorkerToRegistryStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        string id = (string)argv[0];
        IWorker wrk = (IWorker)argv[1];

        return new AddWorkerToRegistryCommand(id: id, wrk: wrk);
    }
}

class AddWorkerToRegistryCommand : ICommand
{
    private string id;
    private IWorker wrk;

    public AddWorkerToRegistryCommand(string id, IWorker wrk)
    {
        this.id = id;
        this.wrk = wrk;
    }

    public void Run()
    {
        var registry = Container.Resolve<IDictionary<string, IWorker>>(
            "Workers.Registry"
        );

        if (registry.ContainsKey(key: this.id))
            throw new Exception("Worker with this id is already exist");
        registry.Add(key: this.id, value: this.wrk);
    }
}
