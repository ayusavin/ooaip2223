namespace SpaceBattleTests.Entities.Strategies;

using Moq;
using SpaceBattle.Entities.Strategies;

public class DelegateStrategyTests
{
    [Fact(Timeout = 1000)]
    public void DelegateStrategy_ExecutesDelegate_Successful()
    {
        // Arrange
        var expectedResult = new object();
        var expectedArgs = new object[] { "test", 42 };
        var delegateMock = new Mock<Func<object[], object>>();
        delegateMock.Setup(d => d(It.Is<object[]>(args => 
            args.Length == expectedArgs.Length && 
            args[0] == expectedArgs[0] && 
            args[1] == expectedArgs[1]
        ))).Returns(expectedResult);

        var strategy = new DelegateStrategy(delegateMock.Object);

        // Act
        var result = strategy.Run(expectedArgs);

        // Assert
        Assert.Same(expectedResult, result);
        delegateMock.Verify(d => d(It.IsAny<object[]>()), Times.Once);
    }

    [Fact(Timeout = 1000)]
    public void DelegateStrategy_WithEmptyArgs_Successful()
    {
        // Arrange
        var expectedResult = new object();
        var delegateMock = new Mock<Func<object[], object>>();
        delegateMock.Setup(d => d(It.Is<object[]>(args => args.Length == 0)))
            .Returns(expectedResult);

        var strategy = new DelegateStrategy(delegateMock.Object);

        // Act
        var result = strategy.Run();

        // Assert
        Assert.Same(expectedResult, result);
        delegateMock.Verify(d => d(It.IsAny<object[]>()), Times.Once);
    }

    [Fact(Timeout = 1000)]
    public void DelegateStrategy_DelegateThrowsException_PropagatesException()
    {
        // Arrange
        var expectedException = new Exception("Test exception");
        var delegateMock = new Mock<Func<object[], object>>();
        delegateMock.Setup(d => d(It.IsAny<object[]>()))
            .Throws(expectedException);

        var strategy = new DelegateStrategy(delegateMock.Object);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => strategy.Run());
        Assert.Same(expectedException, exception);
    }
}
