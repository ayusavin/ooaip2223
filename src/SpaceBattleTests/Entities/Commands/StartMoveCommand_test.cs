namespace SpaceBattleTests.Entities.Commands;

using SpaceBattle.Entities.Commands;
using SpaceBattle.Collections;
using SpaceBattle.Base;
using SpaceBattle.Base.Collections;

using System;

using Moq;

public class StartMoveCommandTests
{
    static StartMoveCommandTests()
    {
        Container.Resolve<ICommand>(
            "Scopes.Current.Set", 
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        Container.Resolve<ICommand>("IoC.Register", "Object.SetupProperty", typeof(SetupPropertyStrategy)).Run();
        Container.Resolve<ICommand>("IoC.Register", "Entities.Adapters.IMovable", typeof(MovableAdapterInjectStrategy)).Run();
        Container.Resolve<ICommand>("IoC.Register", "Entities.Commands.MoveCommand", typeof(CommandInjectStrategy)).Run();
        Container.Resolve<ICommand>("IoC.Register", "Collections.Queue.Push", typeof(QueuePushStrategy)).Run();
    }

    [Fact(Timeout = 1000)]
    public void StartMoveCommand_Success()
    {
        // Init test dependencies
        var MoveCmdStartable = new Mock<IMoveCommandStartable>();
        var UObjectMock = new Mock<IUObject>();
        List<int> Velocity = new List<int> { 42 };

        UObjectMock.SetupSet(uo => uo[It.IsAny<string>()] = Velocity).Verifiable();

        MoveCmdStartable.SetupGet(mcs => mcs.Queue).Returns(new FakeQueue<ICommand>());
        MoveCmdStartable.SetupGet(mcs => mcs.Velocity).Returns(Velocity);
        MoveCmdStartable.SetupGet(mcs => mcs.UObject).Returns(UObjectMock.Object);

        // Create StartMoveCommand
        var smc = new StartMoveCommand(MoveCmdStartable.Object);

        // Action
        smc.Run();

        // Assertation
        Assert.NotNull(MoveCmdStartable.Object.Queue.Pop());
        UObjectMock.Verify();
    }

    [Fact(Timeout = 1000)]
    public void StartMoveCommand_StartableIsNull_Failed()
    {
        // Init test dependencies
        List<int> Velocity = new List<int> { 42 };

        // Create StartMoveCommand
        var smc = new StartMoveCommand(null!);


        // Assertation
        Assert.ThrowsAny<Exception>(() => smc.Run());
    }
}



class SetupPropertyStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        var target = (IUObject)argv[0];
        var value = argv[2];

        var SetupPropertyCommand = new Mock<ICommand>();
        SetupPropertyCommand.Setup(spc => spc.Run()).Callback(new Action(() => target[It.IsAny<string>()] = value));

        return SetupPropertyCommand.Object;
    }
}

class MovableAdapterInjectStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        var Mover = new Mock<IMovable>();

        return Mover.Object;
    }
}

class CommandInjectStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        var cmd = new Mock<ICommand>();
        return cmd.Object;
    }
}

class QueuePushStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        var q = (IQueue<ICommand>)argv[0];
        var val = (ICommand)argv[1];

        var QueuePusher = new Mock<ICommand>();

        QueuePusher.Setup(qp => qp.Run()).Callback(new Action(() => q.Push(val)));

        return QueuePusher.Object;
    }
}

class FakeQueue<T> : IQueue<T>
{
    private T? cmd = default(T);
    public T Pop()
    {
        var returnValue = cmd;
        cmd = default(T)!;

        return returnValue!;
    }

    public void Push(T elem)
    {
        this.cmd = elem;
    }
}
