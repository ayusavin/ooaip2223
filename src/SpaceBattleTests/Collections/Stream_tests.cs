namespace SpaceBattleTests.Collections;

using System.Collections.Concurrent;
using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;

public class StreamTests
{

    [Fact(Timeout = 1000)]
    public void Stream_IsCorrect()
    {

        // Init deps
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Entities.Adapter.IPullable",
            (object[] argv) =>
            {
                var collection = (IProducerConsumerCollection<ICommand>)argv[0];

                var pullable = new Mock<IPullable<ICommand>>();
                pullable.Setup(p => p.Pull()).Returns(() => collection.First());

                return pullable.Object;
            }
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Entities.Adapter.IPushable",
            (object[] argv) =>
            {
                var collection = (IProducerConsumerCollection<ICommand>)argv[0];

                var pushable = new Mock<IPushable<ICommand>>();
                pushable.Setup(p => p.Push(It.IsAny<ICommand>())).Callback(
                    (ICommand cmd) =>
                    {
                        collection.TryAdd(item: cmd);
                    }
                );

                return pushable.Object;
            }
        ).Run();

        var collection = new ConcurrentQueue<ICommand>();

        var stream = new Stream<ICommand>(collection: collection);

        var cmd = new Mock<ICommand>().Object;

        // Action
        stream.Pushable.Push(cmd);

        // Assertation
        Assert.Same(cmd, stream.Pullable.Pull());
    }
}
