namespace SpaceBattleTests.Entities.Commands;

using System;
using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;

public class CollisionCheckCommandTests
{

    private string collisionKey = "Events.Collision.Determinant";

    [Fact(Timeout = 1000)]
    void CollisionCheck_Conflict_Succesful()
    {
        // Init dependencies
        var First = new Mock<IUObject>();
        var Second = new Mock<IUObject>();

        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();
        Container.Resolve<ICommand>("IoC.Register", collisionKey, typeof(TrueCollisionDeterminantStrategy)).Run();

        // Create target
        var ccm = new CollisionCheckCommand(First.Object, Second.Object);

        // Assert
        Assert.ThrowsAny<Exception>(() => ccm.Run());
    }

    [Fact(Timeout = 1000)]
    void CollisionCheck_NoConflict_Succesful()
    {
        // Init dependencies
        var First = new Mock<IUObject>();
        var Second = new Mock<IUObject>();

        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();
        Container.Resolve<ICommand>("IoC.Register", collisionKey, typeof(FalseCollisionDeterminantStrategy)).Run();

        // Create target
        var ccm = new CollisionCheckCommand(First.Object, Second.Object);

        ccm.Run();
    }

    [Fact(Timeout = 1000)]
    void CollisionCheck_NullFirst_ThrowsException()
    {
        // Init dependencies
        var Second = new Mock<IUObject>();

        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();
        Container.Resolve<ICommand>("IoC.Register", collisionKey, typeof(NullCheckCollisionCommand)).Run();

        // Create target
        var ccm = new CollisionCheckCommand(null!, Second.Object);

        Assert.ThrowsAny<Exception>(() => ccm.Run());
    }

    void CollisionCheck_NullSecond_ThrowsException()
    {
        // Init dependencies
        var First = new Mock<IUObject>();

        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();
        Container.Resolve<ICommand>("IoC.Register", collisionKey, typeof(NullCheckCollisionCommand)).Run();

        // Create target
        var ccm = new CollisionCheckCommand(First.Object, null!);

        Assert.ThrowsAny<Exception>(() => ccm.Run());
    }
}

class FalseCollisionDeterminantStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        return false;
    }
}

class TrueCollisionDeterminantStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        return true;
    }
}

class NullCheckCollisionCommand : IStrategy
{
    public object Run(params object[] argv)
    {
        if (argv[0] is null || argv[1] is null)
        {
            throw new NullReferenceException();
        }
        return 0;
    }
}
