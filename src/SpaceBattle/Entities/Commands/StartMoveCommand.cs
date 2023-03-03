namespace SpaceBattle.Entities.Commands;
using SpaceBattle.Base;
using SpaceBattle.Collections;

// Description:
//      Initializes a long running IUObject motion command.
// Parametres:
//      Constructor takes next parametres:
//          IMoveCommandStartable mcs:
//              The object that is the target to start moving.
// Container dependencies:
//      ICommand Object.SetupProperty (IUObject, string key, object value):
//          Command that sets the IUObject value with the given key.
//      
//          Returns:
//              Command that sets the value when the Run() method is called.
//      
//      IMovable Entities.Adapter.IMovable(IMoveCommandStartable):
//          An adapter that casts an IMoveCommandStartable to an IMovable interface.
//      
//          Returns:
//              Returns an IMovable-adapted object.
//
//      ICommand Entities.Commands.MoveCommand(IMovable):
//          Command that performs the movement of IMovable object.
//          
//          Returns:
//              A command that perform the movement of the object on
//              Run() method call.
//      
//      ICommand Collections.Queue.Push (IQueue, ICommand):
//          The command that pushes the command (argv[1]) to the given queue (argv[0]).
//      
//      Parametres:
//          IQueue<ICommand> argv[0]:
//              The queue to which the command will be added.
//          ICommand argv[1]:
//              The command to be added to the queue.
//
//      Returns:
//          ICommand:
//              The command that, when the Run() method is called, will add the command(argv[1]) to the queue(argv[0]).
public class StartMoveCommand : ICommand
{
    private IMoveCommandStartable mcs;

    public StartMoveCommand(IMoveCommandStartable mcs)
    {
        this.mcs = mcs;
    }

    public void Run()
    {
        Container.Resolve<ICommand>("Object.SetupProperty", mcs.UObject, "velocity", mcs.Velocity).Run();

        var Movable = Container.Resolve<IMovable>("Entities.Adapters.IMovable", mcs);

        var MoveCommand = Container.Resolve<ICommand>("Entities.Commands.MoveCommand", Movable);

        Container.Resolve<ICommand>("Collections.Queue.Push", mcs.Queue, MoveCommand).Run();
    }
}
