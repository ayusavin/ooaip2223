namespace SpaceBattle.Entities.Strategies;

using SpaceBattle.Base;
using SpaceBattle.Collections;

class InitOponnentsSpaceshipsStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        var initialPos = (IList<int>)argv[0];
        var distanceBetweenMates = (IList<int>)argv[1];
        var distanceBetweenOpponents = (IList<int>)argv[2];
        var spaceships = (IEnumerable<object>)argv[3];

        return new InitOpponentsSpaceshipsCommand(initialPos, distanceBetweenMates, distanceBetweenOpponents, spaceships);
    }
}

class InitOpponentsSpaceshipsCommand : ICommand
{
    private IList<int> initialPos;
    private IList<int> distanceBetweenMates;
    private IList<int> distanceBetweenOpponents;
    private IEnumerable<object> gameObjects;

    public InitOpponentsSpaceshipsCommand(IList<int> initialPos, IList<int> distanceBetweenMates, IList<int> distanceBetweenOpponents, IEnumerable<object> gameObjects)
    {
        this.initialPos = initialPos;
        this.distanceBetweenMates = distanceBetweenMates;
        this.distanceBetweenOpponents = distanceBetweenOpponents;
        this.gameObjects = gameObjects;
    }

    public void Run()
    {
        ((ICommand)new LinearPositionGeneratorStrategy().Run(this.gameObjects, this.initialPos, this.distanceBetweenMates)).Run();
        ((ICommand)new LinearPositionGeneratorStrategy()
            .Run(
                this.gameObjects,
                Container.Resolve<IList<int>>("Math.IList.Int32.Addition", this.initialPos, this.distanceBetweenOpponents),
                this.distanceBetweenMates)
        ).Run();
    }
}
