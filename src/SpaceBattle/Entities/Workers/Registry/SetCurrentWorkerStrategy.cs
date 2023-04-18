namespace SpaceBattle.Entities.Workers.Registry;

using System.Threading;
using SpaceBattle.Base;
using SpaceBattle.Collections;

public class SetCurrentWorkerStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        IWorker wrk = (IWorker)argv[0];

        return new SetCurrentWorkerCommand(wrk: wrk);
    }
}

class SetCurrentWorkerCommand : ICommand
{

    [ThreadStatic]
    static IWorker currentWorker = null!;

    private IWorker wrk;

    static SetCurrentWorkerCommand()
    {
        Container.Resolve<ICommand>("IoC.Register", "Workers.Current", (object[] _) => currentWorker).Run();
    }

    public SetCurrentWorkerCommand(IWorker wrk)
    {
        this.wrk = wrk;
    }

    public void Run()
    {
        currentWorker = this.wrk;
    }
}
