namespace SpaceBattleTests.Entities.Strategies;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Strategies;

public class EmptyObjectCreateStrategyTests
{
    [Fact(Timeout = 1000)]
    void EmptyObjectCreateStrategy_CreatesEmptyDictionary()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var eocs = new EmptyObjectCreateStrategy();

        Assert.True(((IDictionary<string, object>)eocs.Run()).Count == 0);
    }
}
