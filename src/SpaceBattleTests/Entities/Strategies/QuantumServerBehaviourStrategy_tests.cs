namespace SpaceBattleTests.Entities.Strategies;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Strategies;

public class QuantumServerBehaviourStrategyTests
{

    [Fact(Timeout = 1000)]
    public void QuantumServerBehaviour_EndAllCmdsBeforeTimeout_Succesful()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Workers.Behaviour.Time.Quantum.Ms",
            (object[] _) => (object)Int64.MaxValue
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Exception.Handle",
            (object[] _) => { throw new Exception(); return _; }
        ).Run();

        var tasks = new Queue<ICommand>();
        var pullable = new Mock<IPullable<ICommand>>();

        pullable.Setup(p => p.Pull()).Returns(
            () =>
            {
                try
                {
                    return tasks.Dequeue();
                }
                catch (Exception)
                {
                    return null!;
                }
            }
        );
        pullable.Setup(p => p.Empty()).Returns(
            () =>
            {
                return tasks.Count == 0;
            }
        );

        foreach (int _ in Enumerable.Range(0, 2))
            tasks.Enqueue(new Mock<ICommand>().Object);

        var tsbs = new QuantumServerBehaviourStrategy();

        // Assertation
        Assert.ThrowsAny<Exception>(() => tsbs.Run(pullable.Object));
    }

    [Fact(Timeout = 1000)]
    public void QuantumServerBehaviour_DontCallTasksAfterQuantum_Succesful()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Workers.Behaviour.Time.Quantum.Ms",
            (object[] _) => (object)(long)5
        ).Run();

        var tasks = new Queue<ICommand>();
        var pullable = new Mock<IPullable<ICommand>>();

        pullable.Setup(p => p.Pull()).Returns(
            () =>
            {
                return tasks.Dequeue();
            }
        );
        pullable.Setup(p => p.Empty()).Returns(
            () =>
            {
                return tasks.Count == 0;
            }
        );

        var longCommand = new Mock<ICommand>();
        longCommand.Setup(c => c.Run()).Callback(() => { Thread.Sleep(millisecondsTimeout: 10); });

        tasks.Enqueue(longCommand.Object);
        tasks.Enqueue(new Mock<ICommand>().Object);

        var tsbs = new QuantumServerBehaviourStrategy();

        // Action
        tsbs.Run(pullable.Object);

        // Assertation
        Assert.Equal(tasks.Count, 1);
    }
}
