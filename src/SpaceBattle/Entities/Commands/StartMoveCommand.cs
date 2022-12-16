namespace SpaceBattle.Entities.Commands;
using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;

public class StartMoveCommand : ICommand
{
    private IMoveCommandStartable mcs;

    public StartMoveCommand(IMoveCommandStartable mcs)
    {
        this.mcs = mcs;
    }

    public void Run()
    {
        var container = ServiceLocator.Locate<IContainer>("IoC");
        container.Resolve<ICommand>("Object.SetupProperty", mcs.UObject, "velocity", mcs.Velocity).Run();

        var Movable = container.Resolve<IMovable>("Entities.Adapters.IMovable", mcs);

        var MoveCommand = container.Resolve<ICommand>("Entities.Commands.MoveCommand", Movable);

        container.Resolve<ICommand>("Collections.Queue.Push", mcs.Queue, MoveCommand).Run();
    }
}
