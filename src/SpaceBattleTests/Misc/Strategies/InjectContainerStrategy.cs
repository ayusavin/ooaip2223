namespace SpaceBattleTests.Misc.Strategies;
using SpaceBattle.Collections;
using SpaceBattle.Base;

public class InjectContainerStrategy<ContainerType> : IStrategy
{
    ContainerType Container = Activator.CreateInstance<ContainerType>();
    public object Run(params object[] argv)
    {
        return Container!;
    }
}
