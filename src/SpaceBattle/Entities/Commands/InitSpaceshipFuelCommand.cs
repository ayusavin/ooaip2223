namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;
using SpaceBattle.Base.Collections;

using SpaceBattle.Collections;

public class InitSpaceshipFuelCommand : ICommand
{
    private IEnumerable<object> objs;

    public InitSpaceshipFuelCommand(IEnumerable<object> objs)
    {
        this.objs = objs;
    }

    public void Run()
    {
        var fuel = Container.Resolve<IEnumerator<int>>("Game.Generators.Fuel", objs);

        foreach (object obj in objs)
        {
            IFuelable fuelable = Container.Resolve<IFuelable>("Entities.Adapter.IFuelable", obj);

            fuel.MoveNext();
            fuelable.Level = fuel.Current;
        }
    }
}
