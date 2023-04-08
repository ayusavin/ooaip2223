namespace SpaceBattleTests.Entities.Workers.Stream;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Workers.Stream;

public class StreamRegistryStrategyTests
{

    [Fact(Timeout = 1000)]
    public void GetStream_Successful()
    {

        // Init deps
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var srs = new StreamRegistryStrategy();

        // Action
        ((ICommand)srs.Run()).Run();

        // Asserts
        Assert.NotNull(
            Container.Resolve<IDictionary<string, IStream<ICommand>>>("Workers.Stream.Registry")
        );
    }
}
