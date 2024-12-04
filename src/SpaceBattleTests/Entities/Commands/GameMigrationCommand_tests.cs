namespace SpaceBattleTests.Entities.Commands;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;

public class GameMigrationCommandTests
{
    [Fact(Timeout = 1000)]
    public void GameMigrationCommand_MigrateGameWithPendingCommands_Successful()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        // Setup location registry
        var registry = new Dictionary<string, string>
        {
            { "game1", "thread1" }
        };
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Game.Location.Registry",
            (object[] _) => registry
        ).Run();

        // Setup source stream
        var sourceStream = new Mock<IStream<ICommand>>();
        var sourcePullable = new Mock<IPullable<ICommand>>();
        var sourcePushable = new Mock<IPushable<ICommand>>();

        // Setup target stream
        var targetStream = new Mock<IStream<ICommand>>();
        var targetPushable = new Mock<IPushable<ICommand>>();

        // Setup test commands
        var gameCommand = new GameCommand(new Mock<ICommand>().Object, "game1");
        var otherCommand = new GameCommand(new Mock<ICommand>().Object, "game2");

        // Setup source stream behavior
        var commands = new Queue<ICommand>(new[] { gameCommand, otherCommand });
        sourcePullable.Setup(p => p.Pull()).Returns(() => commands.Count > 0 ? commands.Dequeue() : null);

        // Verify commands are pushed to correct streams
        sourcePushable.Setup(p => p.Push(It.IsAny<ICommand>())).Callback((ICommand cmd) =>
        {
            Assert.Equal(otherCommand, cmd);
        });

        targetPushable.Setup(p => p.Push(It.IsAny<ICommand>())).Callback((ICommand cmd) =>
        {
            Assert.Equal(gameCommand, cmd);
        });

        sourceStream.SetupGet(s => s.Pullable).Returns(sourcePullable.Object);
        sourceStream.SetupGet(s => s.Pushable).Returns(sourcePushable.Object);
        targetStream.SetupGet(s => s.Pushable).Returns(targetPushable.Object);

        // Register streams
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Workers.Stream.Get",
            (object[] argv) =>
            {
                string threadId = (string)argv[0];
                return threadId == "thread1" ? sourceStream.Object : targetStream.Object;
            }
        ).Run();

        // Create and run command
        var cmd = new GameMigrationCommand(
            gameId: "game1",
            sourceThreadId: "thread1",
            targetThreadId: "thread2"
        );
        cmd.Run();

        // Assert
        Assert.Equal("thread2", registry["game1"]);
        sourcePushable.Verify(p => p.Push(It.IsAny<ICommand>()), Times.Once);
        targetPushable.Verify(p => p.Push(It.IsAny<ICommand>()), Times.Once);
    }

    [Fact(Timeout = 1000)]
    public void GameMigrationCommand_MigrateGameWithoutPendingCommands_Successful()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        // Setup location registry
        var registry = new Dictionary<string, string>
        {
            { "game1", "thread1" }
        };
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Game.Location.Registry",
            (object[] _) => registry
        ).Run();

        // Setup streams
        var sourceStream = new Mock<IStream<ICommand>>();
        var targetStream = new Mock<IStream<ICommand>>();
        var sourcePullable = new Mock<IPullable<ICommand>>();

        sourcePullable.Setup(p => p.Pull()).Returns(() => null);
        sourceStream.SetupGet(s => s.Pullable).Returns(sourcePullable.Object);

        // Register streams
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Workers.Stream.Get",
            (object[] argv) =>
            {
                string threadId = (string)argv[0];
                return threadId == "thread1" ? sourceStream.Object : targetStream.Object;
            }
        ).Run();

        // Create and run command
        var cmd = new GameMigrationCommand(
            gameId: "game1",
            sourceThreadId: "thread1",
            targetThreadId: "thread2"
        );
        cmd.Run();

        // Assert
        Assert.Equal("thread2", registry["game1"]);
        sourcePullable.Verify(p => p.Pull(), Times.Once);
    }
} 