namespace SpaceBattle.Entities.Strategies;

using System.Collections;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;

public class InitOponnentsSpaceshipsStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        var spaceshipsFirst = (IEnumerable<object>)argv[0];
        var spaceshipsSecond = (IEnumerable<object>)argv[1];

        return new InitOpponentsSpaceshipsCommand(spaceshipsFirst, spaceshipsSecond);
    }
}

class InitOpponentsSpaceshipsCommand : ICommand
{
    private IEnumerable<object> gameObjectsFirst;
    private IEnumerable<object> gameObjectsSecond;

    public InitOpponentsSpaceshipsCommand(
        IEnumerable<object> gameObjectsFirst,
        IEnumerable<object> gameObjectsSecond
    )
    {
        this.gameObjectsFirst = gameObjectsFirst;
        this.gameObjectsSecond = gameObjectsSecond;
    }

    public void Run()
    {
        Container.Resolve<ICommand>("Game.Objects.Commands.InitPosition", gameObjectsFirst).Run();
        Container.Resolve<ICommand>("Game.Objects.Commands.InitPosition", gameObjectsSecond).Run();
    }
}
