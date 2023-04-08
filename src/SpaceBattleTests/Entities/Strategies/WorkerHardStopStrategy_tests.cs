namespace SpaceBattleTests.Entities.Strategies;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Strategies;

public class WorkerHardStopStrategyTests
{

    public void WorkerHardStop_WithoutCallback_Succesful()
    {
        // Init test dependencies
        var pushable = new Mock<IPushable<ICommand>>();
        pushable.Setup(p => p.Push(It.IsAny<ICommand>())).Verifiable();
        var worker = new Mock<IWorker>();
        string id = "42";

        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        Container.Resolve<ICommand>("IoC.Register", "Workers.Registry.Get", (object[] argv) => worker.Object).Run();

        // Create WorkerHardStopStrategy
        var whss = new WorkerHardStopStrategy();

        // Action
        ((ICommand)whss.Run(id)).Run();

        // Assertation
        pushable.Verify();
    }

    public void WorkerHardStop_WithCallback_Succesful()
    {
        // Init test dependencies
        var pushable = new Mock<IPushable<ICommand>>();
        pushable.Setup(p => p.Push(It.IsAny<ICommand>())).Verifiable();
        var worker = new Mock<IWorker>();
        string id = "42";

        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        Container.Resolve<ICommand>("IoC.Register", "Workers.Registry.Get", (object[] argv) => worker.Object).Run();

        var callback = new Mock<Action>();
        callback.Setup(c => c()).Verifiable();
        // Create WorkerHardStopStrategy
        var whss = new WorkerHardStopStrategy();

        // Action
        ((ICommand)whss.Run(id)).Run();

        // Assertation
        pushable.Verify();
        callback.Verify();
    }
}
