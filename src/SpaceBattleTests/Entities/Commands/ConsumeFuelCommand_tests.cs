using Moq;
using SpaceBattle.Base;
using SpaceBattle.Entities.Commands;

namespace SpaceBattleTests.Entities.Commands;

public class ConsumeFuelCommandTests
{
    [Fact(Timeout = 1000)]
    public void Run_ValidFuelConsumption_Successful()
    {
        // Arrange
        var mockFuelable = new Mock<IFuelable>();
        mockFuelable.SetupProperty(f => f.Level, 10);
        mockFuelable.SetupGet(f => f.BurnSpeed).Returns(5);
        var command = new ConsumeFuelCommand(mockFuelable.Object);

        // Act
        command.Run();

        // Assert
        Assert.Equal(5, mockFuelable.Object.Level);
    }

    [Fact(Timeout = 1000)]
    public void Run_NotEnoughFuel_ThrowsException()
    {
        // Arrange
        var mockFuelable = new Mock<IFuelable>();
        mockFuelable.SetupProperty(f => f.Level, 3);
        mockFuelable.SetupGet(f => f.BurnSpeed).Returns(5);
        var command = new ConsumeFuelCommand(mockFuelable.Object);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => command.Run());
        Assert.Equal(3, mockFuelable.Object.Level); // Level should remain unchanged
    }

    [Fact(Timeout = 1000)]
    public void Run_NonPositiveBurnSpeed_ThrowsException()
    {
        // Arrange
        var mockFuelable = new Mock<IFuelable>();
        mockFuelable.SetupProperty(f => f.Level, 10);
        mockFuelable.SetupGet(f => f.BurnSpeed).Returns(0);
        var command = new ConsumeFuelCommand(mockFuelable.Object);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => command.Run());
        Assert.Equal(10, mockFuelable.Object.Level); // Level should remain unchanged

        // Test negative burn speed
        mockFuelable.SetupGet(f => f.BurnSpeed).Returns(-1);
        Assert.Throws<InvalidOperationException>(() => command.Run());
        Assert.Equal(10, mockFuelable.Object.Level);
    }
}