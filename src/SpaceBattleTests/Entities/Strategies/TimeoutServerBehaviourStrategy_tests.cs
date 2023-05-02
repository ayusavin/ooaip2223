namespace SpaceBattleTests.Entities.Strategies;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Strategies;

public class TimeoutServerBehaviourStrategyTests
{

    [Fact(Timeout = 1000)]
    public void TimeoutServerBehaviour_EndBeforeTimeout_Succesful()
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
            "Workers.Behaviour.Timeout",
            (object[] _) => (object)Int32.MaxValue
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Exception.Handle",
            (object[] _) => { throw new Exception(); return _; }
        ).Run();

        var tasks = new Mock<IPullable<ICommand>>();
        tasks.Setup(t => t.Pull()).Returns(new Mock<ICommand>().Object);

        var tsbs = new TimeoutServerBehaviourStrategy();

        // Assertation
        tsbs.Run(tasks.Object);
    }

    [Fact(Timeout = 1000)]
    public void TimeoutServerBehaviour_Timeout_Happends()
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
            "Workers.Behaviour.Timeout",
            (object[] _) => (object)0
        ).Run();

        var verifier = new Mock<ICommand>();
        verifier.Setup(v => v.Run()).Verifiable();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Exception.Handle",
            (object[] argv) =>
            {
                verifier.Object.Run();

                return new Mock<IStrategy>().Object;
            }
        ).Run();

        var tasks = new Mock<IPullable<ICommand>>();
        var cmd = new Mock<ICommand>();
        cmd.Setup(c => c.Run()).Callback(() => { while (true) { }; });

        tasks.Setup(t => t.Pull()).Returns(cmd.Object);

        var tsbs = new TimeoutServerBehaviourStrategy();

        // Action
        tsbs.Run(tasks.Object);

        // Assertation
        verifier.Verify();
    }
}
