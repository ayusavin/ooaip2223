namespace SpaceBattleTests.Entities.Commands;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;



public class PullableBehaviourCommandTests
{
    [Fact(Timeout = 1000)]
    void ExecutesBehaviourAgainstPullable_Successful()
    {
        // Init dependencies
        var behaviour = new Mock<IStrategy>();
        var pullable = new Mock<IPullable<ICommand>>();

        behaviour.Setup(b => b.Run(It.IsAny<object[]>())).Callback(
            (object[] argv) =>
            {
                var Pullable = (IPullable<ICommand>)argv[0];

                Assert.Same(Pullable, pullable.Object);
            }
        );

        var pbc = new PullableBehaviourCommand(pullable.Object, behaviour.Object);

        // Assertation
        pbc.Run();
    }
}
