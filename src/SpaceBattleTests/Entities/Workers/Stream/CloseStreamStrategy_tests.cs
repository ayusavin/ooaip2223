namespace SpaceBattleTests.Entities.Workers.Stream;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Workers.Stream;

public class CloseStreamStrategyTests
{

    [Fact(Timeout = 1000)]
    public void StreamClosed_WithoutLastCommand_Successful()
    {

        // Init deps
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var registry = new Dictionary<string, IStream<ICommand>>();
        Container.Resolve<ICommand>("IoC.Register", "Workers.Stream.Registry", (object[] _) => registry).Run();

        Container.Resolve<ICommand>("IoC.Register", "Workers.Stream.Push",
            (object[] _) => new Mock<ICommand>().Object
        ).Run();

        var stream = new Mock<IStream<ICommand>>().Object;
        string id = "42";

        var anotherStream = new Mock<IStream<ICommand>>().Object;

        registry.Add(key: id, value: stream);

        var css = new CloseStreamStrategy();

        // Action
        ((ICommand)css.Run(id)).Run();

        // Asserts
        Assert.NotSame(registry.TryGetValue(key: id, value: out anotherStream),
            stream);
    }

    [Fact(Timeout = 1000)]
    public void StreamClosed_WithLastCommand_Successful()
    {

        // Init deps
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var registry = new Dictionary<string, IStream<ICommand>>();
        Container.Resolve<ICommand>("IoC.Register", "Workers.Stream.Registry", (object[] _) => registry).Run();

        var stream = new Mock<IStream<ICommand>>().Object;
        string id = "42";

        var streamBackend = new Mock<ICommand>().Object;

        Container.Resolve<ICommand>("IoC.Register", "Workers.Stream.Push",
            (object[] argv) =>
            {
                string ID = (string)argv[0];
                ICommand lastCmd = (ICommand)argv[1];

                var pushCmd = new Mock<ICommand>();
                if (ID == id)
                    pushCmd.Setup(c => c.Run()).Callback(
                        () =>
                        {
                            streamBackend = lastCmd;
                        }
                    );
                return pushCmd.Object;
            }
        ).Run();


        var anotherStream = new Mock<IStream<ICommand>>().Object;

        registry.Add(key: id, value: stream);

        var lastCmd = new Mock<ICommand>().Object;
        var css = new CloseStreamStrategy();

        // Action
        ((ICommand)css.Run(id, lastCmd)).Run();

        // Asserts
        Assert.NotSame(registry.TryGetValue(key: id, value: out anotherStream),
            stream);
        Assert.Equal(lastCmd, streamBackend);
    }

}
