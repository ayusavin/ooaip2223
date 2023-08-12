namespace SpaceBattleTests.Entities.Generators;

using System.Collections;
using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Generators;

public class GameObjectsFuelGeneratorTests
{
    [Fact(Timeout = 1000)]
    void GameObjectsFuelGenerator_Traverse_Successful()
    {
        // Init deps
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var objects = new List<object>{
            new object(),
            new object(),
            new object()
        };

        var fuelDict = new Dictionary<object, int>();

        foreach (object obj in objects)
        {
            fuelDict[obj] = Random.Shared.Next();
        }

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Game.Objects.Properties.Get",
            (object[] argv) =>
            {
                var propName = (string)argv[0];
                var obj = argv[1];

                if (propName == "Fuel")
                    return (object)fuelDict[obj];
                else throw new Exception();
            }
        ).Run();

        var gofg = new GameObjectsFuelGenerator(objects);

        // Action
        foreach (object obj in objects)
        {
            gofg.MoveNext();
            Assert.Equal(fuelDict[obj], gofg.Current);
            Assert.Equal(fuelDict[obj], ((IEnumerator)gofg).Current);
        }

    }

    [Fact(Timeout = 1000)]
    void GameObjectsFuelGenerator_ResetSuccessful()
    {
        // Init deps
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var objects = new List<object>{
            new object()
        };

        var fuelDict = new Dictionary<object, int>();

        foreach (object obj in objects)
        {
            fuelDict[obj] = Random.Shared.Next();
        }

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Game.Objects.Properties.Get",
            (object[] argv) =>
            {
                var propName = (string)argv[0];
                var obj = argv[1];

                if (propName == "Fuel")
                    return (object)fuelDict[obj];
                else throw new Exception();
            }
        ).Run();

        var gofg = new GameObjectsFuelGenerator(objects);

        // Action
        gofg.MoveNext();
        var first = gofg.Current;
        gofg.Reset();
        gofg.MoveNext();
        var second = gofg.Current;

        // Assertation
        Assert.Equal(first, second);
    }

    [Fact(Timeout = 1000)]
    void GameObjectsFuelGenerator_DisposeSuccessful()
    {
        // Init deps
        var gofg = new GameObjectsFuelGenerator(new object[] { });

        // Assert, don't throw exception, don't do anything
        gofg.Dispose();
    }

    [Fact(Timeout = 1000)]
    void GameObjectsFuelGenerator_MoveNextAfterEnd_ReturnsFalse()
    {
        // Init deps
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var objects = new List<object> { };

        var gofg = new GameObjectsFuelGenerator(objects);

        // Assertation
        Assert.False(gofg.MoveNext());
    }
}
