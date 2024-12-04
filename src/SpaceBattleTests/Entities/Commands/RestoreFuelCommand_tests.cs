using Moq;
using SpaceBattle.Base;
using SpaceBattle.Entities.Commands;

namespace SpaceBattleTests.Entities.Commands;

public class RestoreFuelCommandTests
{
    [Fact(Timeout = 1000)]
    public void Run_RestoresFuel_Successful()
    {
        // Arrange
        var mockFuelable = new Mock<IFuelable>();
        mockFuelable.SetupProperty(f => f.Level, 5);
        var command = new RestoreFuelCommand(mockFuelable.Object, 3);

        // Act
        command.Run();

        // Assert
        Assert.Equal(8, mockFuelable.Object.Level);
    }

    [Fact(Timeout = 1000)]
    public void Run_RestoreNegativeAmount_ReducesFuel()
    {
        // Arrange
        var mockFuelable = new Mock<IFuelable>();
        mockFuelable.SetupProperty(f => f.Level, 10);
        var command = new RestoreFuelCommand(mockFuelable.Object, -3);

        // Act
        command.Run();

        // Assert
        Assert.Equal(7, mockFuelable.Object.Level);
    }
}