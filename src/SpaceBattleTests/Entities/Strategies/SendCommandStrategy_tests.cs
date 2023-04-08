namespace SpaceBattleTests.Entities.Strategies;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Strategies;

public class SendCommandStrategyTests
{

    [Fact(Timeout = 1000)]
    public void SendCommand_Succesful()
    {
        // Init test dependencies
        var stream = new Mock<IStream<ICommand>>();
        var pushable = new Mock<IPushable<ICommand>>();
        var cmd = new Mock<ICommand>();
        string id = "42";

        pushable.Setup(p => p.Push(It.IsAny<ICommand>())).Callback((ICommand Cmd) => { if (Cmd != cmd.Object) throw new Exception(); });

        stream.SetupGet(s => s.Pushable).Returns(pushable.Object);

        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        Container.Resolve<ICommand>("IoC.Register", "Workers.Stream.Get", (object[] argv) =>
        {
            if ((string)argv[0] == id)
                return stream.Object;
            throw new Exception();
        }).Run();

        // Create SendCommandStrategy
        var scs = new SendCommandStrategy();

        // Assertation
        ((ICommand)scs.Run(id, cmd.Object)).Run();
    }
}
