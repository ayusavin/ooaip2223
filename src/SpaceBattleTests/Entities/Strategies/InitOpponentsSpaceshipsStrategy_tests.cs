namespace SpaceBattle.Entities.Strategies;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Strategies;

public class InitOpponentsSpaceshipsStrategyTests
{
    [Fact(Timeout = 1000)]
    void InitOpponentsSpaceshipstrategy_returnsGenerator()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var initCommand = new Mock<ICommand>();
        initCommand.Setup(c => c.Run()).Verifiable();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Game.Objects.Commands.InitPosition",
            (object[] argv) =>
            {
                var gameObjects = (IEnumerable<object>)argv[0];
                return initCommand.Object;
            }
        ).Run();

        var gameObjectsFirst = new List<object> { new object(), new object() };
        var gameObjectsSecond = new List<object> { new object(), new object() };

        var ioss = new InitOponnentsSpaceshipsStrategy();

        // Action
        ((ICommand)ioss.Run(gameObjectsFirst, gameObjectsSecond)).Run();

        // Asertation
        initCommand.Verify();
    }
}
