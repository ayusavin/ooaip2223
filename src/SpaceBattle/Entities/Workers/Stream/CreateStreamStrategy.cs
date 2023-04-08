namespace SpaceBattle.Entities.Workers.Stream;

using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;

public class CreateStreamStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        string id = (string)argv[0];

        return new CreateStreamCommand(id: id);
    }
}

class CreateStreamCommand : ICommand
{
    private string id;

    public CreateStreamCommand(string id)
    {
        this.id = id;
    }

    public void Run()
    {
        var registry = Container.Resolve<IDictionary<string, IStream<ICommand>>>("Workers.Stream.Registry");
        if (registry.ContainsKey(key: this.id))
            throw new Exception("Stream with this id is already exist");
        registry.Add(key: this.id, value: Container.Resolve<IStream<ICommand>>("Workers.Stream.Object"));
    }
}
