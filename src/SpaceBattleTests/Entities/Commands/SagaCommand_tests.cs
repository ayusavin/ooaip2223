using Moq;
using SpaceBattle.Base;
using SpaceBattle.Entities.Commands;

namespace SpaceBattleTests.Entities.Commands;

public class SagaCommandTests
{
    private static List<T> RepeatFactory<T>(int count, Func<T> factory) => Enumerable.Range(0, count).Select(_ => factory()).ToList();

    // Check that all forward commands are executed
    // and no backward commands are executed
    [Fact(Timeout = 1000)]
    public void Run_ExecutesAllForwardCommands_Successful()
    {
        var forwardCommands = RepeatFactory(3, () => new Mock<ICommand>());
        var backwardCommands = RepeatFactory(3, () => new Mock<ICommand>());

        var transactionPairs = forwardCommands
            .Select(c => c.Object)
            .Zip(
                backwardCommands.Select(c => c.Object),
                (f, b) => new TransactionPair(f, b)
            );

        var sagaCommand = new SagaCommand(transactionPairs);

        // Act
        sagaCommand.Run();

        // Assert
        forwardCommands.ForEach(mock => mock.Verify(m => m.Run(), Times.Once));
        backwardCommands.ForEach(mock => mock.Verify(m => m.Run(), Times.Never));
    }

    // Check that all forward commands are executed
    // and in case if latest forward command fails
    // all backward commands except the latest one are executed
    [Fact(Timeout = 1000)]
    public void Run_ExecutesBackwardCommandsOnException_Successful()
    {
        // Arrange
        var mock1 = new Mock<ICommand>();
        var mock2 = new Mock<ICommand>();
        var mockFail = new Mock<ICommand>();
        mockFail.Setup(m => m.Run()).Throws(new Exception("Test exception"));

        var backMock1 = new Mock<ICommand>();
        var backMock2 = new Mock<ICommand>();
        var backMock3 = new Mock<ICommand>();

        var transactionPairs = new List<TransactionPair>
        {
            new TransactionPair(mock1.Object, backMock1.Object),
            new TransactionPair(mock2.Object, backMock2.Object),
            new TransactionPair(mockFail.Object, backMock3.Object)
        };

        var sagaCommand = new SagaCommand(transactionPairs);

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => sagaCommand.Run());

        mock1.Verify(m => m.Run(), Times.Once);
        mock2.Verify(m => m.Run(), Times.Once);
        mockFail.Verify(m => m.Run(), Times.Once);

        backMock1.Verify(m => m.Run(), Times.Once);
        backMock2.Verify(m => m.Run(), Times.Once);
        backMock3.Verify(m => m.Run(), Times.Never);
    }

    [Fact(Timeout = 1000)]
    public void Run_EmptyTransactionPairs_Successful()
    {
        // Arrange
        var sagaCommand = new SagaCommand(new List<TransactionPair>());

        // Act & Assert
        sagaCommand.Run(); // Should not throw
    }

    [Fact(Timeout = 1000)]
    public void Run_BackwardCommandThrowsException_PropagatesException()
    {
        // Arrange
        var forwardCommands = RepeatFactory(2, () => new Mock<ICommand>());
        var backwardCommands = RepeatFactory(2, () => new Mock<ICommand>());

        forwardCommands[1].Setup(m => m.Run()).Throws<Exception>();
        backwardCommands[0].Setup(m => m.Run()).Throws<Exception>();

        var transactionPairs = forwardCommands
            .Select(c => c.Object)
            .Zip(
                backwardCommands.Select(c => c.Object),
                (f, b) => new TransactionPair(f, b)
            );

        var sagaCommand = new SagaCommand(transactionPairs);

        // Act & Assert
        Assert.Throws<AggregateException>(() => sagaCommand.Run());
        forwardCommands[0].Verify(m => m.Run(), Times.Once);
        forwardCommands[1].Verify(m => m.Run(), Times.Once);
        backwardCommands[0].Verify(m => m.Run(), Times.Once);
        backwardCommands[1].Verify(m => m.Run(), Times.Never);
    }
}
