namespace SpaceBattleTests.Entities.Commands;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;

public class GameCommandTests
{

    [Fact(Timeout = 1000)]
    public void GameCommand_ExecuteCommandInGameContext_Successful()
    {
        // Init test dependencies
        var testScope = Container.Resolve<object>("Scopes.New", Container.Resolve<object>("Scopes.Root"));

        string id = "42";
        var gameScope = Container.Resolve<object>("Scopes.New", testScope);
        string testDependencyName = "Test.Dependency";

        // Setup game context dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            gameScope
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            testDependencyName,
            (object[] _) => _
        ).Run();

        // Back to test scope
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            testScope
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Game.Scope.ById",
            (object[] argv) =>
            {
                var ID = (string)argv[0];

                if (id != ID) throw new Exception();

                return gameScope;
            }
        ).Run();

        var task = new Mock<ICommand>();
        task.Setup(t => t.Run()).Callback(() =>
        {
            Container.Resolve<object>(testDependencyName);
        });

        var gameCmd = new GameCommand(task: task.Object, gameId: id);

        // Action
        gameCmd.Run();

        // Assertation
        Assert.Same(
            Container.Resolve<object>("Scopes.Current"),
            testScope
        );
    }
}
