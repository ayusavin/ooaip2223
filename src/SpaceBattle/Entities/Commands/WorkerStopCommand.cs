namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;
using SpaceBattle.Collections;

public class WorkerStopCommand : ICommand
{

    IWorker wrk;

    ICommand callback;

    public WorkerStopCommand(IWorker worker, ICommand callback = null!)
    {
        this.wrk = worker;
        this.callback = callback;
    }

    public void Run()
    {
        if (Container.Resolve<IWorker>("Workers.Current") == this.wrk)
            this.wrk.Stop();
        else throw new Exception("cannot stop this worker");

        if (this.callback is not null)
            this.callback.Run();
    }
}
