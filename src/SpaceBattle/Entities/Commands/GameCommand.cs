namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;
using SpaceBattle.Collections;

public class GameCommand : ICommand
{
    string GameId;
    ICommand task;

    public GameCommand(string GameId, ICommand task)
    {
        this.GameId = GameId;
        this.task = task;
    }

    public void Run()
    {
        // Save current context
        var currentScope = Container.Resolve<object>("Scopes.Current");

        // Entrypoint into game context
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Game.Scope.ById", GameId
            )
        ).Run();

        task.Run();

        // Back to saved context
        Container.Resolve<ICommand>("Scopes.Current.Set", currentScope).Run();
    }
}
