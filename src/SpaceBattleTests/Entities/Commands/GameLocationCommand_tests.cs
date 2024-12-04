namespace SpaceBattleTests.Entities.Commands;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;

public class GameLocationCommandTests
{
    [Fact(Timeout = 1000)]
    public void GameLocationCommand_AddLocation_Successful()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var registry = new Dictionary<string, string>();
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Game.Location.Registry",
            (object[] _) => registry
        ).Run();

        string gameId = "game1";
        string threadId = "thread1";

        // Create and run command
        var cmd = new GameLocationCommand(gameId: gameId, threadId: threadId);
        cmd.Run();

        // Assert
        Assert.Equal(threadId, registry[gameId]);
    }

    [Fact(Timeout = 1000)]
    public void GameLocationCommand_UpdateLocation_Successful()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var registry = new Dictionary<string, string>
        {
            { "game1", "thread1" }
        };
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Game.Location.Registry",
            (object[] _) => registry
        ).Run();

        string gameId = "game1";
        string newThreadId = "thread2";

        // Create and run command
        var cmd = new GameLocationCommand(gameId: gameId, threadId: newThreadId);
        cmd.Run();

        // Assert
        Assert.Equal(newThreadId, registry[gameId]);
    }

    [Fact(Timeout = 1000)]
    public void GameLocationCommand_RemoveLocation_Successful()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var registry = new Dictionary<string, string>
        {
            { "game1", "thread1" }
        };
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Game.Location.Registry",
            (object[] _) => registry
        ).Run();

        string gameId = "game1";
        string threadId = "thread1";

        // Create and run command
        var cmd = new GameLocationCommand(gameId: gameId, threadId: threadId, isRemove: true);
        cmd.Run();

        // Assert
        Assert.False(registry.ContainsKey(gameId));
    }
} 