namespace SpaceBattleTests.Entities.Commands;
using SpaceBattleTests.Attributes;

using SpaceBattle.Entities.Commands;
using SpaceBattle.Collections;
using SpaceBattle.Base;

using System;

using Moq;

public class RotateCommandTests
{

    static RotateCommandTests()
    {
        Container.Resolve<ICommand>(
            "Scopes.Current.Set", 
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        Container.Resolve<ICommand>("IoC.Register", "Math.IList.IAngle.Addition", typeof(AngleVectorAdditionStrategy)).Run();
    }

    [Fact(Timeout = 100)]
    public void RealAngleTest()
    {
        // Prepare
        var CurrentAngleMock = new Mock<IAngle>();
        var ReturnAngleMock = new Mock<IAngle>();

        ReturnAngleMock.SetupGet(a => a.Numerator).Returns(135);
        ReturnAngleMock.SetupGet(a => a.Denominator).Returns(360);

        CurrentAngleMock.Setup(
            a => a.Add(It.IsAny<IAngle>())
        ).Returns(ReturnAngleMock.Object);

        var RotatableMock = new Mock<IRotatable>();

        RotatableMock.SetupGet(r => r.Rotation).Returns(new IAngle[] { CurrentAngleMock.Object });

        RotatableMock.SetupGet(r => r.RotationSpeed).Returns(new IAngle[] { CurrentAngleMock.Object });

        var Rotater = new RotateCommand(RotatableMock.Object);

        // Action
        Rotater.Run();

        // Assertation
        Assert.Equal(135, RotatableMock.Object.Rotation[0].Numerator);
        Assert.Equal(360, RotatableMock.Object.Rotation[0].Denominator);
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
    public void SuccessfulRotationTests(int cur_rot_num, int cur_rot_denom, int rot_speed_num, int rot_speed_denom)
    {
        // Prepare
        var Mock = new Mock<IRotatable>();
        Mock.SetupProperty(Rot => Rot.Rotation, new TestAngle[] { new TestAngle(cur_rot_num, cur_rot_denom) });
        Mock.SetupGet(Rot => Rot.RotationSpeed).Returns(new TestAngle[] { new TestAngle(rot_speed_num, rot_speed_denom) });

        var Roter = new RotateCommand(Mock.Object);

        // Action
        Roter.Run();

        // Assertation
        var Expected = new TestAngle(cur_rot_num + rot_speed_num, cur_rot_denom + rot_speed_denom);
        var Actual = Mock.Object.Rotation[0];

        Assert.Equal(Expected, Actual);
    }

    [Theory(Timeout = 1000)]
    [Repeat(50)]
    public void RandomSuccessfulRotationTests(int _)
    {

        // Initialization
        Random rand = new Random();

        var CurrentRotation = new TestAngle[]{new TestAngle(rand.Next(int.MinValue, int.MaxValue),
                               rand.Next(int.MinValue, int.MaxValue))};

        var RotationSpeed = new TestAngle[]{new TestAngle(rand.Next(int.MinValue, int.MaxValue),
                               rand.Next(int.MinValue, int.MaxValue))};

        // Prepare
        var Mock = new Mock<IRotatable>();
        Mock.SetupProperty(Rot => Rot.Rotation, CurrentRotation.Clone());
        Mock.SetupGet(Rot => Rot.RotationSpeed).Returns((IAngle[])RotationSpeed.Clone());

        var Rotater = new RotateCommand(Mock.Object);

        // Action
        Rotater.Run();

        // Assertation
        var Expected = new TestAngle[]{ new TestAngle(
            CurrentRotation[0].Numerator + RotationSpeed[0].Numerator,
            CurrentRotation[0].Denominator + RotationSpeed[0].Denominator
            )};
        Assert.Equal(Expected, Mock.Object.Rotation);
    }

    [Fact(Timeout = 1000)]
    public void ZeroSizeVectorsTest()
    {
        // Prepare
        var Mock = new Mock<IRotatable>();
        Mock.SetupProperty(Rot => Rot.Rotation, new TestAngle[] { });
        Mock.SetupGet(Rot => Rot.RotationSpeed).Returns(new TestAngle[] { });

        var Rotater = new RotateCommand(Mock.Object);

        // Action
        Rotater.Run();

        // Assertation
        Assert.Equal(new TestAngle[] { }, Mock.Object.Rotation);
    }

    [Fact(Timeout = 1000)]
    public void NullPositionRotateTest()
    {
        // Prepare
        var Mock = new Mock<IRotatable>();
        Mock.SetupProperty(Rot => Rot.Rotation, null);
        Mock.SetupGet(Rot => Rot.RotationSpeed).Returns(new TestAngle[]{
            new TestAngle(0, 1)
        });

        var Rotater = new RotateCommand(Mock.Object);

        // Assertation
        Assert.Throws<NullReferenceException>(() => Rotater.Run());
    }

    [Fact(Timeout = 1000)]
    public void NullRotateSpeedTest()
    {
        // Prepare
        var Mock = new Mock<IRotatable>();
        Mock.SetupProperty(Rot => Rot.Rotation, new TestAngle[]{
            new TestAngle(0, 1)
        });
        Mock.SetupGet(Rot => Rot.RotationSpeed).Returns((TestAngle[]?)null!);

        var Rotater = new RotateCommand(Mock.Object);

        // Assertation
        Assert.Throws<NullReferenceException>(() => Rotater.Run());
    }

    [Fact(Timeout = 1000)]
    public void NullRotateSpeedAndPositionTest()
    {
        // Prepare
        var Mock = new Mock<IRotatable>();
        Mock.SetupProperty(Rot => Rot.Rotation, null);
        Mock.SetupGet(Rot => Rot.RotationSpeed).Returns((TestAngle[]?)null!);

        var Rotater = new RotateCommand(Mock.Object);

        // Assertation
        Assert.Throws<NullReferenceException>(() => Rotater.Run());
    }
}

//  Description:
//          The TestAngle class implements operations sufficient for testing RotateCommand,
//           while not guaranteeing the correctness of algebraic operations on fractions
class TestAngle : IAngle
{
    private int num;
    private int denom;

    public TestAngle(int num, int denom)
    {
        this.Numerator = num;
        this.Denominator = denom;
    }

    public TestAngle()
    {
        this.Numerator = 0;
        this.Denominator = 1;
    }

    public int Numerator { get => this.num; set => this.num = value; }
    public int Denominator { get => this.denom; set => this.denom = value; }

    public IAngle Add(IAngle other)
    {
        TestAngle result = new TestAngle();

        result.Numerator = this.Numerator + other.Numerator;
        result.Denominator = this.Denominator + other.Denominator;

        return result;
    }

    // override object.Equals
    public override bool Equals(object obj)
    {

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        TestAngle other = (TestAngle)obj;

        return this.Numerator == other.Numerator && this.Denominator == other.Denominator;
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}


class AngleVectorAdditionStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        var left = (IList<IAngle>)argv[0];
        var right = (IList<IAngle>)argv[1];

        if (left is null || right is null)
        {
            throw new NullReferenceException();
        }

        for (int i = 0, Size = left.Count; i < Size; i++)
        {
            left[i] = left[i].Add(right[i]);
        }

        return left;
    }
}
