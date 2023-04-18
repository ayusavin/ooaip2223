namespace SpaceBattleTests.Entities.Workers.Registry;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Workers.Registry;

public class SetCurrentWorkerStrategyTests
{

    [Fact(Timeout = 1000)]
    public void SetCurrentWorker_WorkerInitialized_Successful()
    {

        // Init deps
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        IWorker wrk = new Mock<IWorker>().Object;

        var scw = new SetCurrentWorkerStrategy();

        // Action
        ((ICommand)scw.Run(wrk)).Run();

        // Assertation
        Assert.Same(
            wrk,
            Container.Resolve<IWorker>("Workers.Current")
        );
    }

}
