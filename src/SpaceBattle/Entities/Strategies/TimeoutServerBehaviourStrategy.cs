namespace SpaceBattle.Entities.Strategies;

using SpaceBattle.Base;
using SpaceBattle.Collections;

public class TimeoutServerBehaviourStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        IPullable<ICommand> tasks = (IPullable<ICommand>)argv[0];

        // Millisecs
        int timeout = Container.Resolve<int>("Workers.Behaviour.Timeout");

        ICommand cmd = tasks.Pull();
        try
        {
            var task = Task.Run(() => cmd.Run());
            if (!task.Wait(TimeSpan.FromMilliseconds(timeout)))
            {
                throw new TimeoutException();
            }
        }
        catch (Exception e)
        {
            Container.Resolve<IStrategy>("Exception.Handle").Run(cmd, e);
        }

        return 0;
    }
}
