namespace SpaceBattleTests.Entities.Workers.Registry;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Workers.Registry;

public class AddWorkerToRegistryStrategyTests
{

    [Fact(Timeout = 1000)]
    public void WorkerAdd_Successful()
    {

        // Init deps

        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var registry = new Dictionary<string, IWorker>();

        Container.Resolve<ICommand>("IoC.Register", "Workers.Registry", (object[] _) => registry).Run();

        string id = "42";
        IWorker wrk = new Mock<IWorker>().Object;

        var aws = new AddWorkerToRegistryStrategy();

        // Action
        ((ICommand)aws.Run(id, wrk)).Run();

        // Asserts
        Assert.True(registry[id] == wrk);
    }

    [Fact(Timeout = 1000)]
    public void WorkerAdd_Failed_WorkerAlreadyExist()
    {
        // Init deps

        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var registry = new Dictionary<string, IWorker>();

        Container.Resolve<ICommand>("IoC.Register", "Workers.Registry", (object[] _) => registry).Run();

        string id = "42";
        IWorker wrk = new Mock<IWorker>().Object;
        IWorker anotherWrk = new Mock<IWorker>().Object;

        registry.Add(key: id, value: anotherWrk);

        var aws = new AddWorkerToRegistryStrategy();

        // Assert
        Assert.ThrowsAny<Exception>(() => ((ICommand)aws.Run(id, wrk)).Run());
    }
}
