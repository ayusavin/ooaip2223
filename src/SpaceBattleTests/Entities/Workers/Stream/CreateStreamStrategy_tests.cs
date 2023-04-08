namespace SpaceBattleTests.Entities.Workers.Stream;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Workers.Stream;

public class CreateStreamStrategyTests
{

    [Fact(Timeout = 1000)]
    public void CreateStream_Successful()
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

        string id = "42";

        var css = new CreateStreamStrategy();

        // Action
        ((ICommand)css.Run(id)).Run();

        // Asserts
        Assert.NotNull(registry[id]);
    }

}
