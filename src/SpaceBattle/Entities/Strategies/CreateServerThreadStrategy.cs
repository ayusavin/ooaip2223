namespace SpaceBattle.Entities.Strategies;

using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;
using SpaceBattle.Entities.Workers;

public class CreateServerThreadStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        string id = (string)argv[0];
        ICommand action = argv.Count() > 1 ? (ICommand)argv[1] : new EmptyCommand();

        return new CreateServerThreadCommand(id: id, action: action);
    }
}

class CreateServerThreadCommand : ICommand
{
    string id;
    ICommand action;

    public CreateServerThreadCommand(string id, ICommand action)
    {
        this.id = id;
        this.action = action;
    }

    public void Run()
    {
        // Create worker tasks stream
        Container.Resolve<ICommand>(
            "Workers.Stream.Create",
            id
        ).Run();

        IStream<ICommand> stream =
        Container.Resolve<IStream<ICommand>>(
            "Workers.Stream.Get",
            id
        );

        // Create worker,
        IWorker worker = new ServerThread(Stream: stream.Pullable);

        // Register worker
        Container.Resolve<ICommand>("Workers.Registry.Add", id, worker).Run();

        this.action.Run();
    }
}
