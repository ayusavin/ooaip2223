namespace SpaceBattleTests.Entities.Strategies;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Strategies;

public class DefaultExceptionHandlerStrategyTests
{

    [Fact(Timeout = 1000)]
    void ExceptionHandlerStrategy_rethrowsException()
    {
        // init deps
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var dehs = new DefaultExceptionHandlerStrategy();


        // Assertation
        Assert.Throws<Exception>(() => dehs.Run(
            new Mock<ICommand>().Object,
            new Exception()
        ));
    }
}
