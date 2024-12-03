namespace SpaceBattle.Entities.Strategies;

using SpaceBattle.Base;

public class DelegateStrategy : IStrategy
{
    private readonly Func<object[], object> _delegate;

    public DelegateStrategy(Func<object[], object> @delegate)
    {
        _delegate = @delegate;
    }

    public object Run(params object[] argv)
    {
        return _delegate(argv);
    }
}
