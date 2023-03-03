namespace SpaceBattleTests.Entities.Strategies;

using SpaceBattle.Base;
using SpaceBattle.Entities.Strategies;
using SpaceBattle.Collections;

using Moq;

public class ExceptionHandlerFindStrategyTests {

    [Fact(Timeout = 1000)]
    void ExceptionHandlerFindStrategy_Successful() {
        // Init dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set", 
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();
        Container.Resolve<ICommand>("IoC.Register","Handler.Exception", typeof(DictionaryInjectionStrategy)).Run();

        var ehfs = new ExceptionHandlerFindStrategy();

        // Action
        Assert.NotNull(ehfs.Run(
            new Mock<ICommand>().Object,
            new Mock<Exception>().Object
            ));
    }
}

class DictionaryInjectionStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        var Handler = new Mock<IStrategy>();

        var ExceptionDict = new Mock<IDictionary<Exception, IStrategy>>();
        ExceptionDict.Setup(ed => ed[It.IsAny<Exception>()]).Returns(Handler.Object);

        var MagicDict = new Mock<IDictionary<ICommand, IDictionary<Exception, IStrategy>>>();

        MagicDict.Setup(md => md[It.IsAny<ICommand>()]).Returns(ExceptionDict.Object);

        return MagicDict.Object;
    }
}
