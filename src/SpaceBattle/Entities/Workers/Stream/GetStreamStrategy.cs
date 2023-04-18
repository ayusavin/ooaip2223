namespace SpaceBattle.Entities.Workers.Stream;

using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;

public class GetStreamStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        string id = (string)argv[0];

        return Container.Resolve<IDictionary<string, IStream<ICommand>>>("Workers.Stream.Registry")[id];
    }
}
