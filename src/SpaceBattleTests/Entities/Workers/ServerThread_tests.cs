namespace SpaceBattleTests.Entities.Workers;

using Moq;

using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Workers;

public class ServerThreadTests
{
    [Fact(Timeout = 1000)]
    public void ServerThreadStartStop_Successful()
    {
        // Init dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var mre = new ManualResetEventSlim(false);

        var stream = new Mock<IPullable<ICommand>>();
        var behav = new Mock<IStrategy>();
        behav.Setup(b => b.Run(It.IsAny<object[]>())).Callback(() => mre.Set());

        // Create ServerThread
        IWorker st = new ServerThread(Stream: stream.Object, behaviour: behav.Object);

        Container.Resolve<ICommand>("IoC.Register", "Workers.Current.Set", (object[] argv) => new Mock<ICommand>().Object).Run();
        // CheckBehaviour
        st.Start();

        mre.Wait();

        st.Stop();
    }

    [Fact(Timeout = 1000)]
    public void ServerThreadChangeBehaviour_Successful()
    {
        // Init dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        Container.Resolve<ICommand>("IoC.Register", "Workers.Current.Set", (object[] argv) => new Mock<ICommand>().Object).Run();

        var stream = new Mock<IPullable<ICommand>>();

        // CheckBehaviour
        var mre = new ManualResetEventSlim(false);
        var anotherMre = new ManualResetEventSlim(false);

        var behav = new Mock<IStrategy>();
        behav.Setup(s => s.Run(It.IsAny<Object[]>())).Callback(() => mre.Set());

        var anotherBehav = new Mock<IStrategy>();
        anotherBehav.Setup(s => s.Run(It.IsAny<Object[]>())).Callback(() => anotherMre.Set());

        IWorker st = new ServerThread(Stream: stream.Object, behaviour: behav.Object);

        st.Start();

        mre.Wait();

        st.Behaviour = anotherBehav.Object;

        anotherMre.Wait();

        st.Stop();
    }
}
