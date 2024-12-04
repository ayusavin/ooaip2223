using Moq;
using SpaceBattle.Base;
using SpaceBattle.Entities.Commands;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Strategies;

namespace SpaceBattleTests.Entities.Strategies;

public class BackwardCommandStrategiesTests
{
    [Fact(Timeout = 1000)]
    public void RegisterBackwardCommand_RegistersCommandSuccessfully()
    {
        // Arrange
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var strategy = new RegisterBackwardCommandStrategy();
        var mockCommand = new Mock<ICommand>().Object;
        var forwardCommandName = "TestCommand";

        // Act
        strategy.Run(forwardCommandName, mockCommand);

        // Assert
        var resolvedCommand = Container.Resolve<ICommand>($"BackwardCommand.{forwardCommandName}");
        Assert.Same(mockCommand, resolvedCommand);
    }

    [Fact(Timeout = 1000)]
    public void ResolveBackwardCommand_ExistingCommand_ReturnsCommand()
    {
        // Arrange
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var mockCommand = new Mock<ICommand>().Object;
        var forwardCommandName = "TestCommand";

        Container.Resolve<ICommand>(
            "IoC.Register",
            $"BackwardCommand.{forwardCommandName}",
            (object[] _) => mockCommand
        ).Run();

        var strategy = new ResolveBackwardCommandStrategy(null);

        // Act
        var result = strategy.Run(forwardCommandName);

        // Assert
        Assert.Same(mockCommand, result);
    }

    [Fact(Timeout = 1000)]
    public void ResolveBackwardCommand_NonExistingCommand_ReturnsEmptyCommand()
    {
        // Arrange
        var strategy = new ResolveBackwardCommandStrategy(null);

        // Act
        var result = strategy.Run("NonExistingCommand");

        // Assert
        Assert.IsType<EmptyCommand>(result);
    }
}
