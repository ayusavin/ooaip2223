namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;

public class GameMigrationCommand : ICommand
{
    private readonly string gameId;
    private readonly string sourceThreadId;
    private readonly string targetThreadId;

    public GameMigrationCommand(string gameId, string sourceThreadId, string targetThreadId)
    {
        this.gameId = gameId;
        this.sourceThreadId = sourceThreadId;
        this.targetThreadId = targetThreadId;
    }

    public void Run()
    {
        // Update game location
        new GameLocationCommand(gameId, targetThreadId).Run();

        // Get pending messages for the game
        var sourceStream = Container.Resolve<IStream<ICommand>>("Workers.Stream.Get", sourceThreadId);
        var targetStream = Container.Resolve<IStream<ICommand>>("Workers.Stream.Get", targetThreadId);

        // Transfer any pending game commands to the new thread
        var getCommand = () => sourceStream.Pullable.Pull();
        ICommand cmd = getCommand();
        while (cmd != null)
        {
            if (cmd is GameCommand gameCmd && gameCmd.GameId == gameId)
            {
                targetStream.Pushable.Push(cmd);
            }
            else
            {
                sourceStream.Pushable.Push(cmd);
            }
            cmd = getCommand();
        }
    }
} 