namespace SpaceBattleTests.Entities.Commands;
using SpaceBattleTests.Misc.Strategies;

using SpaceBattle.Entities.Commands;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Base.Collections;
using System.Reflection;

using System;

using Moq;

using TestContainerType = SpaceBattle.Collections.HWDTechContainer;

//[CollectionDefinition("Non-Parallel Collection", DisableParallelization = true)]
public class CollisionCheckCommandTests
{

    private string collisionKey = "Events.Collision.Determinant";

    static CollisionCheckCommandTests()
    {
        try
        {
            ServiceLocator.Register("IoC", new InjectContainerStrategy<TestContainerType>());
        }
        catch (Exception) { }
    }

    [Fact(Timeout = 1000)]
    void CollisionCheck_Conflict_Succesful()
    {
        var container = ServiceLocator.Locate<IContainer>("IoC");

        // Init dependencies
        var First = new Mock<IUObject>();
        var Second = new Mock<IUObject>();

        container.Resolve<ICommand>(
            "Scopes.Current.Set", 
            container.Resolve<object>(
                "Scopes.New", container.Resolve<object>("Scopes.Root")
            )
        ).Run();
        container.Resolve<ICommand>("IoC.Register", collisionKey, typeof(TrueCollisionDeterminantStrategy)).Run();

        // Create target
        var ccm = new CollisionCheckCommand(First.Object, Second.Object);

        // Assert
        Assert.ThrowsAny<Exception>(() => ccm.Run());
    }

    [Fact(Timeout = 1000)]
    void CollisionCheck_NoConflict_Succesful()
    {
        var container = ServiceLocator.Locate<IContainer>("IoC");

        // Init dependencies
        var First = new Mock<IUObject>();
        var Second = new Mock<IUObject>();

        container.Resolve<ICommand>(
            "Scopes.Current.Set", 
            container.Resolve<object>(
                "Scopes.New", container.Resolve<object>("Scopes.Root")
            )
        ).Run();
        container.Resolve<ICommand>("IoC.Register", collisionKey, typeof(FalseCollisionDeterminantStrategy)).Run();

        // Create target
        var ccm = new CollisionCheckCommand(First.Object, Second.Object);

        ccm.Run();
    }

    [Fact(Timeout = 1000)]
    void CollisionCheck_NullFirst_ThrowsException()
    {
        var container = ServiceLocator.Locate<IContainer>("IoC");

        // Init dependencies
        var Second = new Mock<IUObject>();

        container.Resolve<ICommand>(
            "Scopes.Current.Set", 
            container.Resolve<object>(
                "Scopes.New", container.Resolve<object>("Scopes.Root")
            )
        ).Run();
        container.Resolve<ICommand>("IoC.Register", collisionKey, typeof(NullCheckCollisionCommand)).Run();

        // Create target
        var ccm = new CollisionCheckCommand(null!, Second.Object);

        Assert.ThrowsAny<Exception>(() => ccm.Run());
    }

    void CollisionCheck_NullSecond_ThrowsException()
    {
        var container = ServiceLocator.Locate<IContainer>("IoC");

        // Init dependencies
        var First = new Mock<IUObject>();

        container.Resolve<ICommand>(
            "Scopes.Current.Set", 
            container.Resolve<object>(
                "Scopes.New", container.Resolve<object>("Scopes.Root")
            )
        ).Run();
        container.Resolve<ICommand>("IoC.Register", collisionKey, typeof(NullCheckCollisionCommand)).Run();

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
