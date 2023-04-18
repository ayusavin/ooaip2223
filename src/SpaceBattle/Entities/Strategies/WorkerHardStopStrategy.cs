namespace SpaceBattle.Entities.Strategies;

using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;

public class WorkerHardStopStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        string id = (string)argv[0];
        ICommand action = argv.Count() > 1 ? (ICommand)argv[1] : new EmptyCommand();

        return new WorkerHardStopCommand(id: id, action: action);
    }
}

class WorkerHardStopCommand : ICommand
{

    string id;
    ICommand action;

    public WorkerHardStopCommand(string id, ICommand action)
    {
        this.id = id;
        this.action = action;
    }

    public void Run()
    {
        var stream = Container.Resolve<IStream<ICommand>>("Workers.Stream.Get", id);
        stream.Pushable.Push(new WorkerStopCommand(
                worker: Container.Resolve<IWorker>(
                    "Workers.Registry.Get",
                    this.id
                ),
                callback: this.action
            ));
    }
}
