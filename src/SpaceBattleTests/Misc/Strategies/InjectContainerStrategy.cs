namespace SpaceBattleTests.Misc.Strategies;
using SpaceBattle.Collections;
using SpaceBattle.Base;

public class InjectContainerStrategy : IStrategy
{
    private readonly Container cont = new Container();

    public object Run(params object[] argv)
    {
        return cont;
    }
}
