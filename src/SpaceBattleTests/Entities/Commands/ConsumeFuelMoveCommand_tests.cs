using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;
using SpaceBattle.Entities.Strategies;
using SpaceBattle.Entities.Adapters;

namespace SpaceBattleTests.Entities.Commands;

public class ConsumeFuelMoveCommandTests
{
    static ConsumeFuelMoveCommandTests()
    {
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        // Register adapter resolvers
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Entities.Adapter.IMovable",
            (object[] argv) =>
            {
                var obj = (IUObject)argv[0];
                var movable = new Mock<IMovable>();
                movable.SetupGet(m => m.Position).Returns(() => (IList<int>)obj["Position"]);
                movable.SetupGet(m => m.MoveSpeed).Returns(() => (IList<int>)obj["MoveSpeed"]);
                movable.SetupSet(m => m.Position = It.IsAny<IList<int>>())
                    .Callback<IList<int>>(value => obj["Position"] = value);
                return movable.Object;
            }
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Entities.Adapter.IFuelable",
            (object[] argv) =>
            {
                var obj = (IUObject)argv[0];
                var fuelable = new Mock<IFuelable>();
                fuelable.SetupGet(f => f.Level).Returns(() => (int)obj["Fuel"]);
                fuelable.SetupGet(f => f.BurnSpeed).Returns(() => (int)obj["BurnSpeed"]);
                fuelable.SetupSet(f => f.Level = It.IsAny<int>())
                    .Callback<int>(value => obj["Fuel"] = value);
                return fuelable.Object;
            }
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Math.IList.Int32.Addition",
            (object[] argv) =>
            {
                var left = (IList<int>)argv[0];
                var right = (IList<int>)argv[1];

                if (left.Count != right.Count)
                    throw new ArgumentException("Lists must be of equal length");

                return left.Zip(right, (l, r) => l + r).ToList();
            }
        ).Run();

        // Register command factories
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Move",
            (object[] argv) => new MoveCommand(Container.Resolve<IMovable>("Entities.Adapter.IMovable", argv[0]))
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "ConsumeFuel",
            (object[] argv) => new ConsumeFuelCommand(Container.Resolve<IFuelable>("Entities.Adapter.IFuelable", argv[0]))
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "MakeSagaCommand",
            (object[] argv) => new MakeSagaCommandStrategy().Run(argv)
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "BackwardCommand.Move",
            (object[] argv) => new MoveCommand(
                new ReverseMoveAdapter(
                    Container.Resolve<IMovable>("Entities.Adapter.IMovable", argv[0])
                )
            )
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "BackwardCommand.ConsumeFuel",
            (object[] argv) => new RestoreFuelCommand(
                Container.Resolve<IFuelable>("Entities.Adapter.IFuelable", argv[0]),
                Container.Resolve<IFuelable>("Entities.Adapter.IFuelable", argv[0]).BurnSpeed
            )
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "BackwardCommand",
            (object[] argv) =>
            {
                var (commandName, rest) = ((string)argv[0], argv.Skip(1).ToArray());
                return Container.Resolve<ICommand>($"BackwardCommand.{commandName}", rest);
            }
        ).Run();
    }

    [Fact(Timeout = 1000)]
    public void Run_ValidCommand_MovesAndConsumesFuel()
    {
        // Arrange
        var mockUObject = new Mock<IUObject>();
        var position = new[] { 12, 5 };
        var velocity = new[] { 1, 1 };
        var initialFuel = 10;
        var burnSpeed = 1;

        mockUObject.Setup(m => m["Position"]).Returns(position);
        mockUObject.Setup(m => m["MoveSpeed"]).Returns(velocity);
        mockUObject.Setup(m => m["Fuel"]).Returns(initialFuel);
        mockUObject.Setup(m => m["BurnSpeed"]).Returns(burnSpeed);

        var command = new ConsumeFuelMoveCommand(mockUObject.Object);

        // Act
        command.Run();

        // Assert
        mockUObject.VerifySet(m => m["Position"] = It.Is<IList<int>>(p => p[0] == position[0] + velocity[0] && p[1] == position[1] + velocity[1]));
        mockUObject.VerifySet(m => m["Fuel"] = initialFuel - burnSpeed);
    }

    [Fact(Timeout = 1000)]
    public void Run_NotEnoughFuel_RevertsMovement()
    {
        // Arrange
        var mockUObject = new Mock<IUObject>();
        var position = new[] { 12, 5 };
        var velocity = new[] { 1, 1 };
        var initialFuel = 0;
        var burnSpeed = 1;

        mockUObject.Setup(m => m["Position"]).Returns(position);
        mockUObject.Setup(m => m["MoveSpeed"]).Returns(velocity);
        mockUObject.Setup(m => m["Fuel"]).Returns(initialFuel);
        mockUObject.Setup(m => m["BurnSpeed"]).Returns(burnSpeed);

        var command = new ConsumeFuelMoveCommand(mockUObject.Object);

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => command.Run());
        Assert.Equal(position, mockUObject.Object["Position"]);
        Assert.Equal(initialFuel, mockUObject.Object["Fuel"]);
    }
}
