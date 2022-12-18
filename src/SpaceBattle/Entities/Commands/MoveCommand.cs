namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;
using SpaceBattle.Collections;

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
