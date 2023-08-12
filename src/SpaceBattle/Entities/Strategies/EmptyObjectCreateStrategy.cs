namespace SpaceBattle.Entities.Strategies;

using SpaceBattle.Base;

public class EmptyObjectCreateStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        return new Dictionary<string, object>();
    }
}
