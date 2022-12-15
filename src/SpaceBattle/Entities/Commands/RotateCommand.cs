namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;
using SpaceBattle.Base.Collections;
using SpaceBattle.Collections;

public class RotateCommand : ICommand
{

    private IRotatable Rotatable;

    public RotateCommand(IRotatable target)
    {
        this.Rotatable = target;
    }

    public void Run()
    {
        var cont = ServiceLocator.Locate<IContainer>("IoC");
        this.Rotatable.Rotation = cont.Resolve<IList<IAngle>>("Math.IList.IAngle.Addition", this.Rotatable.Rotation, this.Rotatable.RotationSpeed);
    }
}