namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;

public class PullableBehaviourCommand : ICommand
{
    private IPullable<ICommand> src;
    private IStrategy behaviour;

    public PullableBehaviourCommand(IPullable<ICommand> source, IStrategy behaviour)
    {
        this.src = source;
        this.behaviour = behaviour;
    }

    public void Run()
    {
        this.behaviour.Run(this.src);
    }
}
