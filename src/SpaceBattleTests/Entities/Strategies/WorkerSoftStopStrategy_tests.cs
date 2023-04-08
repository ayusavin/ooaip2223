namespace SpaceBattleTests.Entities.Strategies;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Strategies;

public class WorkerSoftStopStrategyTests
{

    public void WorkerSoftStop_WithoutCallback_Succesful()
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

        Container.Resolve<ICommand>("IoC.Register", "Workers.Stream.Close", (object[] argv) =>
        {
            string id = (string)argv[0];
            ICommand lastCommand = argv.Count() > 1 ? (ICommand)argv[1] : null!;
        }).Run();

        // Create WorkerHardStopStrategy
        var wsss = new WorkerSoftStopStrategy();

        // Action
        ((ICommand)wsss.Run(id)).Run();

        // Assertation
        pushable.Verify();
    }

    public void WorkerSoftStop_WithCallback_Succesful()
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


        Container.Resolve<ICommand>("IoC.Register", "Workers.Stream.Close", (object[] argv) =>
        {
            string id = (string)argv[0];
            ICommand lastCommand = argv.Count() > 1 ? (ICommand)argv[1] : null!;
        }).Run();

        var callback = new Mock<Action>();
        callback.Setup(c => c()).Verifiable();
        // Create WorkerHardStopStrategy
        var wsss = new WorkerSoftStopStrategy();

        // Action
        ((ICommand)wsss.Run(id)).Run();

        // Assertation
        pushable.Verify();
        callback.Verify();
    }
}
