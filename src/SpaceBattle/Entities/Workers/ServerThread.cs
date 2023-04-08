namespace SpaceBattle.Entities.Workers;

using System.Threading;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Strategies;

public class ServerThread : IWorker
{
    private IStrategy _behaviour = new DefaultServerBehaviourStrategy();

    public IStrategy Behaviour { get => this._behaviour; set => this._behaviour = value; }

    private Thread thread = null!;

    private bool isRunning = false;

    private IPullable<ICommand> Stream;

    public ServerThread(IPullable<ICommand> Stream)
    {
        this.Stream = Stream;
    }

    public ServerThread(IPullable<ICommand> Stream, IStrategy behaviour)
    {
        this.Stream = Stream;
        this.Behaviour = behaviour;
    }

    public void Start()
    {
        this.isRunning = true;

        var currentScope = Container.Resolve<object>("Scopes.Current");
        this.thread = new Thread(start: () =>
        {
            Container.Resolve<ICommand>(
                "Scopes.Current.Set",
                currentScope
            ).Run();

            Container.Resolve<ICommand>("Workers.Current.Set", this).Run();

            while (this.isRunning)
            {
                this.Behaviour.Run(this.Stream);
            }
        }
        );
        this.thread.Start();
    }

    public void Stop()
    {
        this.isRunning = false;
    }

}
