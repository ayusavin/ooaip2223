namespace SpaceBattleTests.Misc.Strategies;
using SpaceBattle.Base;
using SpaceBattleTests.Misc.Collections;

public class InjectReRegisterableIoC : IStrategy
{
    private readonly ReRegisterableIoC container = new ReRegisterableIoC();
    public object Run(params object[] argv)
    {
        return container;
    }
}
