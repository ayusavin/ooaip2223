using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;
using SpaceBattle.Entities.Strategies;

namespace SpaceBattleTests.Entities.Strategies;

public class MakeSagaCommandStrategyTests
{
    [Fact(Timeout = 1000)]
    public void Run_CreatesSagaCommandWithCorrectTransactionPairs_Successful()
    {
        // Arrange
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var commandNames = new[] { "Command1", "Command2" };
        var uObject = new Mock<IUObject>().Object;

        var forwardCommand1 = new Mock<ICommand>().Object;
        var forwardCommand2 = new Mock<ICommand>().Object;
        var backwardCommand1 = new Mock<ICommand>().Object;
        var backwardCommand2 = new Mock<ICommand>().Object;

        Container.Resolve<ICommand>("IoC.Register", "Command1", (object[] _) => forwardCommand1).Run();
        Container.Resolve<ICommand>("IoC.Register", "Command2", (object[] _) => forwardCommand2).Run();
        Container.Resolve<ICommand>("IoC.Register", "BackwardCommand", (object[] args) =>
            args[0].ToString() == "Command1" ? backwardCommand1 : backwardCommand2
        ).Run();

        var strategy = new MakeSagaCommandStrategy();

        // Act
        var result = strategy.Run(commandNames, uObject) as SagaCommand;

        // Assert
        Assert.NotNull(result);
        var transactionPairs = result.GetType().GetField("transactionPairs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(result) as IEnumerable<TransactionPair>;
        Assert.NotNull(transactionPairs);
        Assert.Equal(2, transactionPairs.Count());
        Assert.Same(forwardCommand1, transactionPairs.ElementAt(0).ForwardCommand);
        Assert.Same(backwardCommand1, transactionPairs.ElementAt(0).BackwardCommand);
        Assert.Same(forwardCommand2, transactionPairs.ElementAt(1).ForwardCommand);
        Assert.Same(backwardCommand2, transactionPairs.ElementAt(1).BackwardCommand);
    }

    [Fact(Timeout = 1000)]
    public void Run_NullCommandNames_ThrowsException()
    {
        // Arrange
        var strategy = new MakeSagaCommandStrategy();
        var uObject = new Mock<IUObject>().Object;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => strategy.Run(null, uObject));
    }
}
