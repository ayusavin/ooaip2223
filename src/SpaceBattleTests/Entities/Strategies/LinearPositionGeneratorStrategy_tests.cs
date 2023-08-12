namespace SpaceBattle.Entities.Strategies;

using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Strategies;

public class LinearPositionGeneratorStrategyTests
{
    [Fact(Timeout = 1000)]
    void LinearPositionGeneratorStrategy_returnsGenerator()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Math.IList.Int32.Addition",
            (object[] argv) =>
            {
                var a = (IList<int>)argv[0];
                var b = (IList<int>)argv[1];

                var result = new List<int>();

                for (int i = 0; i < a.Count; ++i)
                {
                    result.Add(a[i] + b[i]);
                }

                return result;
            }
        ).Run();

        var lpgs = new LinearPositionGeneratorStrategy();

        var iterable = Enumerable.Repeat<object>(new object(), 3);
        var start = new List<int> { 0, 0 };
        var step = new List<int> { 1, 0 };

        var generator = (IEnumerator<IList<int>>)lpgs.Run(iterable, start, step);

        // Action
        generator.MoveNext();

        // Assertation
        Assert.Equal<IList<int>>(generator.Current, new List<int> { 1, 0 });
    }
}
