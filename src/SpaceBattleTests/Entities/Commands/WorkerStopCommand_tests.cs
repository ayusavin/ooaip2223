namespace SpaceBattleTests.Entities.Commands;

using System;
using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;

public class WorkerStopCommandTests
{
    [Fact(Timeout = 1000)]
    public void WorkerStopped_Without_Callback_Succesful()
    {
        // Init test dependencies
        var mock = new Mock<IWorker>();

        mock.Setup(m => m.Stop()).Verifiable();

        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();
        Container.Resolve<ICommand>("IoC.Register", "Workers.Current",
            (object[] argv) => mock.Object).Run();

        // Create WorkerStopCommand
        var wsc = new WorkerStopCommand(worker: mock.Object);

        // Action
        wsc.Run();

        // Assertation
        mock.Verify();
    }

    [Fact(Timeout = 1000)]
    public void WorkerStopped_With_Callback_Succesful()
    {
        // Init test dependencies
        var mock = new Mock<IWorker>();

        mock.Setup(m => m.Stop()).Verifiable();

        var callback = new Mock<ICommand>();
        callback.Setup(c => c.Run()).Verifiable();

        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();
        Container.Resolve<ICommand>("IoC.Register", "Workers.Current",
            (object[] argv) => mock.Object).Run();

        // Create WorkerStopCommand
        var wsc = new WorkerStopCommand(
            worker: mock.Object,
            callback: callback.Object
        );

        // Action
        wsc.Run();

        // Assertation
        callback.Verify();
        mock.Verify();
    }

    [Fact(Timeout = 1000)]
    public void WorkerStopped_Failed()
    {
        // Init test dependencies
        var mock = new Mock<IWorker>();
        var another = new Mock<IWorker>();

        bool mockIsRunning = true;
        bool anotherIsRunning = true;
        mock.Setup(m => m.Stop()).Verifiable();
        another.Setup(m => m.Stop()).Verifiable();

        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();
        Container.Resolve<ICommand>("IoC.Register", "Workers.Current",
           (object[] argv) => another.Object).Run();

        // Create WorkerStopCommand
        var wsc = new WorkerStopCommand(
            worker: mock.Object
        );

        // Assertation
        Assert.ThrowsAny<Exception>(() => wsc.Run());
        Assert.True(mockIsRunning && anotherIsRunning);
    }
}
