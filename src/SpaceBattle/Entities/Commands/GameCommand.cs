namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;
using SpaceBattle.Collections;

public class GameCommand : ICommand
{
    string GameId;

    public GameCommand(string GameId)
    {
        this.GameId = GameId;
    }

    public void Run()
    {
        // Inherit current scope
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Current")
            )
        ).Run();

        var gameWorker = Container.Resolve<IWorker>("Workers.New", GameId);

        gameWorker.Behaviour = Container.Resolve<IStrategy>("Workers.Behaviour.Default");

        gameWorker.Start();
    }
}
