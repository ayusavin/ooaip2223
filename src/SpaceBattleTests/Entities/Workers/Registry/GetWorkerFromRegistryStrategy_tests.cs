namespace SpaceBattleTests.Entities.Workers.Registry;

using Moq;

using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Workers.Registry;

public class GetWorkerFromRegistryStrategyTests
{

    [Fact(Timeout = 1000)]
    public void WorkerExtraction_Successful()
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
        registry.Add(key: id, value: wrk);

        var gws = new GetWorkerFromRegistryStrategy();

        // Asserts
        Assert.Equal(gws.Run(id), wrk);
    }

    [Fact(Timeout = 1000)]
    public void WorkerExtraction_WorkerIsNotExist_Failed()
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

        var gws = new GetWorkerFromRegistryStrategy();

        // Asserts
        Assert.ThrowsAny<Exception>(() => gws.Run(id));
    }

}
