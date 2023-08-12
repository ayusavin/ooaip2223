namespace SpaceBattleTests.Entities.Generators;

using System.Collections;
using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Generators;

public class LinearPositionGeneratorTests
{
    [Fact(Timeout = 1000)]
    void LinearPositionGenerator_Traverse_Successful()
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

        var start = new List<int> { 0, 0 };
        var step = new List<int> { 1, 2 };
        var count = 3;

        var lpg = new LinearPositionGenerator(count: count, start: start, step: step);

        IList<int> current = start;

        // Action
        while (lpg.MoveNext())
        {
            current = Container.Resolve<IList<int>>("Math.IList.Int32.Addition", current, step);
            Assert.Equal(current, lpg.Current);
            Assert.Equal(current, ((IEnumerator)lpg).Current);
        }

    }

    [Fact(Timeout = 1000)]
    void LinearPositionGenerator_ResetSuccessful()
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

        var start = new List<int> { 0, 0 };
        var step = new List<int> { 1, 2 };
        var count = 1;

        var lpg = new LinearPositionGenerator(count: count, start: start, step: step);

        IList<int> current = start;

        // Action
        lpg.MoveNext();
        var first = lpg.Current;
        lpg.Reset();
        lpg.MoveNext();
        var second = lpg.Current;

        // Assertation
        Assert.Equal(first, second);
    }

    [Fact(Timeout = 1000)]
    void LinearPositionGenerator_DisposeSuccessful()
    {
        var start = new List<int> { 0, 0 };
        var step = new List<int> { 1, 2 };
        var count = 1;

        // Init deps
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

        // Init deps
        var lpg = new LinearPositionGenerator(count, start, step);

        // Assert, don't throw exception, don't do anything
        lpg.Dispose();
    }

    [Fact(Timeout = 1000)]
    void LinearPositionGenerator_MoveNextAfterEnd_ReturnsFalse()
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

        var start = new List<int> { 0, 0 };
        var step = new List<int> { 1, 2 };
        var count = 0;

        var lpg = new LinearPositionGenerator(count: count, start: start, step: step);

        // Assertation
        Assert.False(lpg.MoveNext());
    }
}
