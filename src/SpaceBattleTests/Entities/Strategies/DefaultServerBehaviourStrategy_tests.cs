namespace SpaceBattleTests.Entities.Strategies;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Strategies;

public class DefaultServerBehaviourStrategyTests
{

    [Fact(Timeout = 1000)]
    public void DefaultServerBehabiour_ProcessingTask_Succesful()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var tasks = new Mock<IPullable<ICommand>>();
        var task = new Mock<ICommand>();

        tasks.Setup(p => p.Pull()).Returns(task.Object);
        task.Setup(c => c.Run()).Verifiable();

        var dsb = new DefaultServerBehaviourStrategy();

        // Action
        dsb.Run(tasks.Object);

        // Assertation
        task.Verify();
    }

    [Fact(Timeout = 1000)]
    public void DefaultServerBehabiour_HandleException_Succesful()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var tasks = new Mock<IPullable<ICommand>>();
        var task = new Mock<ICommand>();

        tasks.Setup(p => p.Pull()).Returns(task.Object);
        task.Setup(c => c.Run()).Callback(() => throw new Exception());

        var exceptionHandler = new Mock<IStrategy>();
        exceptionHandler.Setup(eh => eh.Run(It.IsAny<object[]>())).Verifiable();

        Container.Resolve<ICommand>("IoC.Register", "Exception.Handle", (object[] _) => exceptionHandler.Object).Run();

        var dsb = new DefaultServerBehaviourStrategy();

        // Action
        dsb.Run(tasks.Object);

        // Assertation
        exceptionHandler.Verify();
    }

}
