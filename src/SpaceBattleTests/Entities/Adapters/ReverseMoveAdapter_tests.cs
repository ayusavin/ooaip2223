using Moq;
using SpaceBattle.Base;
using SpaceBattle.Entities.Adapters;

namespace SpaceBattleTests.Entities.Adapters;

public class ReverseMoveAdapterTests
{
    [Fact(Timeout = 1000)]
    public void Position_GetAndSet_DelegatesCorrectly()
    {
        // Arrange
        var mockMovable = new Mock<IMovable>();
        var position = new List<int> { 1, 2, 3 };
        mockMovable.SetupProperty(m => m.Position, position);
        var adapter = new ReverseMoveAdapter(mockMovable.Object);

        // Act & Assert - Get
        Assert.Equal(position, adapter.Position);

        // Act & Assert - Set
        var newPosition = new List<int> { 4, 5, 6 };
        adapter.Position = newPosition;
        Assert.Equal(newPosition, mockMovable.Object.Position);
    }

    [Fact(Timeout = 1000)]
    public void MoveSpeed_Get_ReturnsReversedSpeed()
    {
        // Arrange
        var mockMovable = new Mock<IMovable>();
        var speed = new List<int> { 1, -2, 3 };
        mockMovable.SetupGet(m => m.MoveSpeed).Returns(speed);
        var adapter = new ReverseMoveAdapter(mockMovable.Object);

        // Act
        var reversedSpeed = adapter.MoveSpeed;

        // Assert
        Assert.Equal(new List<int> { -1, 2, -3 }, reversedSpeed);
    }

    [Fact(Timeout = 1000)]
    public void MoveSpeed_OriginalSpeedChanged_ReturnsNewReversedSpeed()
    {
        // Arrange
        var mockMovable = new Mock<IMovable>();
        var speed = new List<int> { 1, -2, 3 };
        mockMovable.SetupGet(m => m.MoveSpeed).Returns(speed);
        var adapter = new ReverseMoveAdapter(mockMovable.Object);

        // Act
        var firstReversedSpeed = adapter.MoveSpeed;
        speed = new List<int> { 2, 3, 4 };
        mockMovable.SetupGet(m => m.MoveSpeed).Returns(speed);
        var secondReversedSpeed = adapter.MoveSpeed;

        // Assert
        Assert.Equal(new List<int> { -2, -3, -4 }, secondReversedSpeed);
    }
}