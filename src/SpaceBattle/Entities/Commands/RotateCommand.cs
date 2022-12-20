namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;
using SpaceBattle.Collections;

// Description:
//      Implementation of the rotation command.
// Parametres:
//      Constructor takes next parametres:
//          IRotatable target:
//              An object of type IRotatable that will
//              be rotated when the Run() method is called.
// Container dependencies:
//      Math.IList.IAngle.Addition(IList<IAngle>, IList<IAngle>):
//          Adds two collections with IAngle as a vectors.
//          
//          Returns:
//              IList<IAngle>:
//                  Result of addition operation.
public class RotateCommand : ICommand
{

    private IRotatable Rotatable;

    public RotateCommand(IRotatable target)
    {
        this.Rotatable = target;
    }

    public void Run()
    {
        var cont = new Container();
        this.Rotatable.Rotation = cont.Resolve<IList<IAngle>>("Math.IList.IAngle.Addition", this.Rotatable.Rotation, this.Rotatable.RotationSpeed);
    }
}
