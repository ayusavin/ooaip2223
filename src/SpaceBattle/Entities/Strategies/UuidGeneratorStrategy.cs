namespace SpaceBattle.Entities.Strategies;

using SpaceBattle.Base;

public class UuidGeneratorStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        return Guid.NewGuid().ToString();
    }
}
