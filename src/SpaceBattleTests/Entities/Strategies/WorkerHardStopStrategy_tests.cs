namespace SpaceBattleTests.Entities.Strategies;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Strategies;

public class WorkerHardStopStrategyTests
{

    [Fact(Timeout = 1000)]
    public void WorkerHardStop_PushesCommandToStream_WithoutCallback_Succesful()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        string id = "42";

        var wrk = new Mock<IWorker>();
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Workers.Registry.Get",
            (object[] argv) =>
            {
                if ((string)argv[0] != id) throw new Exception();
                return wrk.Object;
            }
        ).Run();

        var pushable = new Mock<IPushable<ICommand>>();
        var stream = new Mock<IStream<ICommand>>();

        stream.SetupGet(s => s.Pushable).Returns(pushable.Object);
        pushable.Setup(p => p.Push(It.IsAny<ICommand>())).Verifiable();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Workers.Stream.Get",
            (object[] argv) =>
            {
                if ((string)argv[0] != id) throw new Exception();
                return stream.Object;
            }
        ).Run();

        var whs = new WorkerHardStopStrategy();

        // Action
        ((ICommand)whs.Run(id)).Run();

        // Assertation
        pushable.Verify();
    }

    [Fact(Timeout = 1000)]
    public void WorkerHardStop_PushesCommandToStream_WithCallback_Succesful()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        string id = "42";

        var wrk = new Mock<IWorker>();
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Workers.Registry.Get",
            (object[] argv) =>
            {
                if ((string)argv[0] != id) throw new Exception();
                return wrk.Object;
            }
        ).Run();

        var pushable = new Mock<IPushable<ICommand>>();
        var stream = new Mock<IStream<ICommand>>();

        stream.SetupGet(s => s.Pushable).Returns(pushable.Object);
        pushable.Setup(p => p.Push(It.IsAny<ICommand>())).Verifiable();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Workers.Stream.Get",
            (object[] argv) =>
            {
                if ((string)argv[0] != id) throw new Exception();
                return stream.Object;
            }
        ).Run();

        var callback = new Mock<ICommand>().Object;

        var whs = new WorkerHardStopStrategy();

        // Action
        ((ICommand)whs.Run(id, callback)).Run();

        // Assertation
        pushable.Verify();
    }
}
