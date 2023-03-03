namespace SpaceBattleTests.Entities.Commands;
using SpaceBattleTests.Attributes;

using SpaceBattle.Entities.Commands;
using SpaceBattle.Collections;
using SpaceBattle.Base;

using System;

using Moq;

public class MoveCommandTests
{
    static MoveCommandTests()
    {
        Container.Resolve<ICommand>(
            "Scopes.Current.Set", 
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        Container.Resolve<ICommand>("IoC.Register", "Math.IList.Int32.Addition", typeof(MoveAdditionStrategy)).Run();
    }

    [Theory(Timeout = 1000)]
    [InlineData(0, 0, 0, 0)]
    [InlineData(0, 0, 1, 2)]
    [InlineData(0, 0, -1, -2)]
    [InlineData(0, 0, -1, 2)]
    [InlineData(0, 0, 1, -2)]
    [InlineData(1, 2, 0, 0)]
    [InlineData(-1, 2, 0, 0)]
    [InlineData(1, -2, 0, 0)]
    [InlineData(-1, -2, 0, 0)]
    [InlineData(1, 2, -2, -1)]
    [InlineData(-3, -4, 1, 2)]
    [InlineData(12, 5, -7, 3)]
    public void SuccessfulMoveTests(int pos_x, int pos_y, int mov_x, int mov_y)
    {
        // Prepare
        var Mock = new Mock<IMovable>();
        Mock.SetupProperty(mov => mov.Position, new int[] { pos_x, pos_y });
        Mock.SetupGet(mov => mov.MoveSpeed).Returns(new int[] { mov_x, mov_y });

        var Mover = new MoveCommand(Mock.Object);

        // Action
        Mover.Run();

        // Assertation
        Assert.Equal(new int[] { pos_x + mov_x, pos_y + mov_y }, Mock.Object.Position);
    }

    [Theory(Timeout = 1000)]
    [Repeat(50)]
    public void RandomSuccessfulMoveTests(int _)
    {
        // Initialization
        Random rand = new Random();

        int[] position = new int[]{rand.Next(int.MinValue, int.MaxValue),
                               rand.Next(int.MinValue, int.MaxValue)};

        int[] moveSpeed = new int[]{rand.Next(int.MinValue, int.MaxValue),
                               rand.Next(int.MinValue, int.MaxValue)};

        // Prepare
        var Mock = new Mock<IMovable>();
        Mock.SetupProperty(mov => mov.Position, position.Clone());
        Mock.SetupGet(mov => mov.MoveSpeed).Returns((int[])moveSpeed.Clone());

        var Mover = new MoveCommand(Mock.Object);

        // Action
        Mover.Run();

        // Assertation
        int[] Expected = new int[] { position[0] + moveSpeed[0], position[1] + moveSpeed[1] };
        Assert.Equal(Expected, Mock.Object.Position);
    }

    [Fact(Timeout = 1000)]
    public void NullPositionMoveTest()
    {
        // Prepare
        var Mock = new Mock<IMovable>();
        Mock.SetupProperty(mov => mov.Position, null);
        Mock.SetupGet(mov => mov.MoveSpeed).Returns(new int[] { 0, 0 });

        var Mover = new MoveCommand(Mock.Object);

        // Assertation
        Assert.Throws<NullReferenceException>(() => Mover.Run());
    }
    [Fact(Timeout = 1000)]
    public void NullMoveSpeedTest()
    {
        // Prepare
        var Mock = new Mock<IMovable>();
        Mock.SetupProperty(mov => mov.Position, new int[] { 0, 0 });
        Mock.SetupGet(mov => mov.MoveSpeed).Returns((int[]?)null!);

        var Mover = new MoveCommand(Mock.Object);

        // Assertation
        Assert.Throws<NullReferenceException>(() => Mover.Run());
    }

    [Fact(Timeout = 1000)]
    public void NullMoveSpeedAndPositionTest()
    {
        // Prepare
        var Mock = new Mock<IMovable>();
        Mock.SetupProperty(mov => mov.Position, null);
        Mock.SetupGet(mov => mov.MoveSpeed).Returns((int[]?)null!);

        var Mover = new MoveCommand(Mock.Object);

        // Assertation
        Assert.Throws<NullReferenceException>(() => Mover.Run());
    }
}

public class MoveAdditionStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        var left = (IList<int>)argv[0];
        var right = (IList<int>)argv[1];

        if (left is null || right is null)
        {
            throw new NullReferenceException();
        }

        for (int i = 0, Size = left.Count(); i < Size; ++i)
        {
            left[i] += right[i];
        }
        return left;
    }
}
