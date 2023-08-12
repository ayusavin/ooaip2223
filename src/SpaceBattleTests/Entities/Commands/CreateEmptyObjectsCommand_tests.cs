namespace SpaceBattleTests.Entities.Commands;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;


public class CreateEmptyObjectsCommandTests
{

    [Theory(Timeout = 1000)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    void CreateEmptyObjects_CreatesNObjects_Successful(int objects_n)
    {
        // Init deps
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        IDictionary<string, object> registry = new Dictionary<string, object>();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Game.Objects.Registry.Add",
            (object[] argv) =>
            {
                string id = (string)argv[0];
                object obj = argv[1];

                var registryAdder = new Mock<ICommand>();
                registryAdder.Setup(c => c.Run()).Callback(() => registry.Add(id, obj));

                return registryAdder.Object;
            }
        ).Run();

        Container.Resolve<ICommand>("IoC.Register", "System.Generator.Uuid", (object[] _) => Guid.NewGuid().ToString()).Run();
        Container.Resolve<ICommand>("IoC.Register", "Game.Objects.Empty", (object[] _) => new object()).Run();

        var ceoc = new CreateEmptyObjectsCommand(objects_n: objects_n);
        ceoc.Run();

        // Assertation
        Assert.Equal(objects_n, registry.Count);
    }
}
