namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;
using SpaceBattle.Collections;

// Description:
//      Implementation of the Linear Motion Command.
// Parametres:
//      Constructor takes next parametres:
//          IMovable target:
//              An object of type IMovable, which will change
//              its position when the Run() method is called.
// Container dependencies:
//      IList<int> Math.IList.Int32.Addition (IList<int>, IList<int>):
//          Adds two collections with System.Int32 as a vectors.
//
//          Returns:
//              IList<int>:
//                  Result of addition operation.
public class MoveCommand : ICommand
{

    private IMovable movable;

    public MoveCommand(IMovable target)
    {
        this.movable = target;
    }

    public void Run()
    {
        var container = new Container();
        this.movable.Position = container.Resolve<IList<int>>("Math.IList.Int32.Addition", movable.Position, movable.MoveSpeed);
    }
}
