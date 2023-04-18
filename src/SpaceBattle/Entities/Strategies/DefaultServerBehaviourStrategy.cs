namespace SpaceBattle.Entities.Strategies;

using SpaceBattle.Base;
using SpaceBattle.Collections;

public class DefaultServerBehaviourStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        IPullable<ICommand> tasks = (IPullable<ICommand>)argv[0];

        ICommand cmd = tasks.Pull();
        try
        {
            cmd.Run();
        }
        catch (Exception e)
        {
            Container.Resolve<IStrategy>("Exception.Handle").Run(cmd, e);
        }

        return 0;
    }
}
