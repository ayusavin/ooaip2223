namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;
using SpaceBattle.Collections;

public class GameLocationCommand : ICommand
{
    private readonly string gameId;
    private readonly string threadId;
    private readonly bool isRemove;

    public GameLocationCommand(string gameId, string threadId, bool isRemove = false)
    {
        this.gameId = gameId;
        this.threadId = threadId;
        this.isRemove = isRemove;
    }

    public void Run()
    {
        var registry = Container.Resolve<IDictionary<string, string>>("Game.Location.Registry");
        
        if (isRemove)
        {
            registry.Remove(gameId);
        }
        else
        {
            registry[gameId] = threadId;
        }
    }
} 