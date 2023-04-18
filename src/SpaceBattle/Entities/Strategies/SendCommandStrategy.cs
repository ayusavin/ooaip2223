namespace SpaceBattle.Entities.Strategies;

using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;

public class SendCommandStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        string id = (string)argv[0];
        ICommand cmd = (ICommand)argv[1];

        return new SendCommandCommand(id: id, cmd: cmd);
    }
}

class SendCommandCommand : ICommand
{

    string id;
    ICommand cmd;

    public SendCommandCommand(string id, ICommand cmd)
    {
        this.id = id;
        this.cmd = cmd;
    }

    public void Run()
    {
        IStream<ICommand> stream =
        Container.Resolve<IStream<ICommand>>(
            "Workers.Stream.Get",
            this.id
        );

        stream.Pushable.Push(this.cmd);
    }
}
