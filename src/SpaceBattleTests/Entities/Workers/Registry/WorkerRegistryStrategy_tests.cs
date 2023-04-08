namespace SpaceBattleTests.Entities.Workers.Registry;

using Moq;

using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Workers.Registry;

public class WorkerRegistryStrategyTests
{

    [Fact(Timeout = 1000)]
    public void WorkerRegistryStrategy_CreatesDependency_Successful()
    {

        // Init deps
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var wrs = new WorkerRegistryStrategy();

        // Action
        ((ICommand)wrs.Run()).Run();

        // Asserts
        Assert.NotNull(
            Container.Resolve<IDictionary<string, IWorker>>("Workers.Registry")
        );
    }

}
