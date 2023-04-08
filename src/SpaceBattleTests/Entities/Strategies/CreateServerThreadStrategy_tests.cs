namespace SpaceBattleTests.Entities.Strategies;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Strategies;

public class CreateServerThreadStrategyTests
{

    [Fact(Timeout = 1000)]
    public void CreateServerThread_WithoutCallback_Succesful()
    {
        // Init test dependencies
        var stream = new Mock<IStream<ICommand>>();
        var pullable = new Mock<IPullable<ICommand>>();
        var streamCreateCmd = new Mock<ICommand>();
        string id = "42";

        stream.SetupGet(s => s.Pullable).Returns(pullable.Object);

        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        Container.Resolve<ICommand>("IoC.Register", "Workers.Stream.Create", (object[] argv) => streamCreateCmd.Object).Run();
        Container.Resolve<ICommand>("IoC.Register", "Workers.Stream.Get", (object[] argv) => stream.Object).Run();

        IDictionary<string, IWorker> Registry = new Dictionary<string, IWorker> { };

        Container.Resolve<ICommand>("IoC.Register", "Workers.Registry.Add",
            (object[] argv) =>
            {
                string id = (string)argv[0];
                IWorker wrk = (IWorker)argv[1];

                var cmd = new Mock<ICommand>();
                cmd.Setup(m => m.Run()).Callback(() => { Registry[id] = wrk; });

                return cmd.Object;
            }).Run();

        // Create WorkerStopCommand
        var csts = new CreateServerThreadStrategy();

        // Action
        ((ICommand)csts.Run(id)).Run();

        // Assertation
        Assert.True(Registry[id] is IWorker);
    }

    [Fact(Timeout = 1000)]
    public void CreateServerThread_WithCallback_Succesful()
    {
        // Init test dependencies
        var stream = new Mock<IStream<ICommand>>();
        var pullable = new Mock<IPullable<ICommand>>();
        var streamCreateCmd = new Mock<ICommand>();
        string id = "42";

        stream.SetupGet(s => s.Pullable).Returns(pullable.Object);

        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        Container.Resolve<ICommand>("IoC.Register", "Workers.Stream.Create", (object[] argv) => streamCreateCmd.Object).Run();
        Container.Resolve<ICommand>("IoC.Register", "Workers.Stream.Get", (object[] argv) => stream.Object).Run();

        IDictionary<string, IWorker> Registry = new Dictionary<string, IWorker> { };

        Container.Resolve<ICommand>("IoC.Register", "Workers.Registry.Add",
            (object[] argv) =>
            {
                string id = (string)argv[0];
                IWorker wrk = (IWorker)argv[1];

                var cmd = new Mock<ICommand>();
                cmd.Setup(m => m.Run()).Callback(() => { Registry[id] = wrk; });

                return cmd.Object;
            }).Run();

        var callback = new Mock<ICommand>();
        callback.Setup(c => c.Run()).Verifiable();

        // Create WorkerStopCommand
        var csts = new CreateServerThreadStrategy();

        // Action
        ((ICommand)csts.Run(id, callback.Object)).Run();

        // Assertation
        Assert.True(Registry[id] is IWorker);
        callback.Verify();
    }
}
