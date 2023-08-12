namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;
using SpaceBattle.Base.Collections;

using SpaceBattle.Collections;

public class InitGameObjectsPositionCommand : ICommand
{
    private IEnumerable<object> objs;

    public InitGameObjectsPositionCommand(IEnumerable<object> objs)
    {
        this.objs = objs;
    }

    public void Run()
    {
        var Position = Container.Resolve<IEnumerator<IList<int>>>("Game.Generators.Position", objs);

        foreach (object obj in objs)
        {
            IMovable movable = Container.Resolve<IMovable>("Entities.Adapter.IMovable", obj);

            Position.MoveNext();
            movable.Position = Position.Current;
        }
    }
}
