namespace SpaceBattleTests.Entities.Workers.Stream;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Workers.Stream;

public class GetStreamStrategyTests
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

        var registry = new Dictionary<string, IStream<ICommand>>();
        Container.Resolve<ICommand>("IoC.Register", "Workers.Stream.Registry", (object[] _) => registry).Run();

        var stream = new Mock<IStream<ICommand>>().Object;
        string id = "42";

        registry.Add(key: id, value: stream);

        var gss = new GetStreamStrategy();

        // Asserts
        Assert.Same(registry[id], gss.Run(id));
    }
}
