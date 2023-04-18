namespace SpaceBattle.Entities.Workers.Stream;

using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;

public class CloseStreamStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        string id = (string)argv[0];
        ICommand lastCommand = argv.Count() > 1 ? (ICommand)argv[1] : new EmptyCommand();

        return new CloseStreamCommand(id: id, lastCommand: lastCommand);
    }
}

class CloseStreamCommand : ICommand
{
    private string id;
    private ICommand lastCommand;

    public CloseStreamCommand(string id, ICommand lastCommand)
    {
        this.id = id;
        this.lastCommand = lastCommand;
    }

    public void Run()
    {
        Container.Resolve<ICommand>("Workers.Stream.Push", id, lastCommand).Run();
        // Naive
        Container.Resolve<IDictionary<string, IStream<ICommand>>>("Workers.Stream.Registry").Remove(key: id);
    }
}
