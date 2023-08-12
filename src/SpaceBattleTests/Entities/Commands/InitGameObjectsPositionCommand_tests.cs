namespace SpaceBattleTests.Entities.Commands;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;

public class InitGameObjectsPositionCommandTests
{

    [Fact(Timeout = 1000)]
    void InitGameObjectsPosition_Successful()
    {
        // Init deps
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        IList<int> generatee = new List<int> { 0 };

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Game.Generators.Position",
            (object[] _) =>
            {
                var generator = new Mock<IEnumerator<IList<int>>>();
                generator.Setup(g => g.MoveNext()).Callback(
                    () =>
                    {
                        generatee[0]++;
                    }
                );
                generator.Setup(g => g.Current).Returns(() => generatee);

                return generator.Object;
            }
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Entities.Adapter.IMovable",
            (object[] argv) =>
            {
                var obj = (IDictionary<string, object>)argv[0];

                var movable = new Mock<IMovable>();
                movable.SetupGet(m => m.Position).Returns(() => (IList<int>)obj["Position"]);
                movable.SetupSet(m => m.Position).Callback((IList<int> value) => { obj["Position"] = value; });

                return movable.Object;
            }
        ).Run();

        var objects = Enumerable.Repeat(new Dictionary<string, object>(), 5);

        var igopc = new InitGameObjectsPositionCommand(objects);

        // Action
        igopc.Run();

        // Assertation
        foreach (IDictionary<string, object> obj in Enumerable.Reverse(objects))
        {
            Assert.True(((IList<int>)obj["Position"]).SequenceEqual(generatee));
            generatee[0]--;
        }
    }
}
