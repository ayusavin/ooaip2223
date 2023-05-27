namespace SpaceBattle.Entities.Strategies;

using SpaceBattle.Base;
using SpaceBattle.Entities.Generators;

public class LinearPositionGeneratorStrategy : IStrategy
{

    public object Run(params object[] argv)
    {
        var iterable = (IEnumerable<object>)argv[0];
        var start = (IList<int>)argv[1];
        var step = (IList<int>)argv[2];

        return new LinearPositionGenerator(count: iterable.Count(), start: start, step: step);
    }
}
