namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;
using SpaceBattle.Collections;

public class WorkerCloseCommand : ICommand
{
    string id;
    ICommand action;

    public WorkerCloseCommand(string id)
    {
        this.id = id;
        this.action = new EmptyCommand();
    }

    public WorkerCloseCommand(string id, ICommand action)
    {
        this.id = id;
        this.action = action;
    }

    public void Run()
    {
        Container.Resolve<ICommand>(
            "Workers.Stream.Close",
            this.id,
            new WorkerStopCommand(
                worker: Container.Resolve<IWorker>(
                    "Workers.Registry.Get",
                    this.id
                ),
                callback: this.action
            )
        ).Run();
    }
}
