namespace SpaceBattleTests.Entities.Commands;

using System.Linq;
using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;

public class InitSpaceshipFuelCommandTests
{
    [Fact(Timeout = 1000)]
    void InitSpaceshipFuelCommand_SetupFuelSuccessful()
    {
        // Init deps
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        int generatee = 0;

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Game.Generators.Fuel",
            (object[] _) =>
            {
                var generator = new Mock<IEnumerator<int>>();
                generator.Setup(g => g.MoveNext()).Callback(
                    () =>
                    {
                        generatee++;
                    }
                );
                generator.Setup(g => g.Current).Returns(() => generatee);

                return generator.Object;
            }
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Entities.Adapter.IFuelable",
            (object[] argv) =>
            {
                var obj = (IDictionary<string, object>)argv[0];

                var fuelable = new Mock<IFuelable>();
                fuelable.SetupGet(f => f.Level).Returns(() => (int)obj["Fuel"]);
                fuelable.SetupSet(f => f.Level).Callback((int value) => { obj["Fuel"] = value; });

                return fuelable.Object;
            }
        ).Run();

        var objects = new List<Dictionary<string, object>>();
        foreach (int _ in Enumerable.Range(0, 5))
        {
            objects.Add(new Dictionary<string, object>());
        }

        var isfc = new InitSpaceshipFuelCommand(objects.AsEnumerable());

        // Action
        isfc.Run();

        // Assertation
        Assert.Equal<object>(
            objects.Select((Dictionary<string, object> obj) => obj["Fuel"]).ToList<object>(),
            Enumerable.Range(1, generatee).ToList<int>()
        );
    }
}
