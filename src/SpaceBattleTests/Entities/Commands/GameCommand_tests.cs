namespace SpaceBattleTests.Entities.Commands;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;

public class GameCommandTests
{

    [Fact(Timeout = 1000)]
    void GameCommand_GameStarts_Successful()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        string id = "42";
        var worker = new Mock<IWorker>();
        worker.Setup(w => w.Start()).Verifiable();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Workers.New",
            (object[] argv) =>
            {
                var ID = (string)argv[0];

                if (id != ID) throw new Exception();

                return worker.Object;
            }
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Workers.Behaviour.Default",
            (object[] _) => new Mock<IStrategy>().Object
        ).Run();

        var gameCmd = new GameCommand(GameId: id);

        // Action
        gameCmd.Run();

        // Assertation
        worker.Verify();
    }
}
