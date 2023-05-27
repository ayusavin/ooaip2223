namespace SpaceBattle.Entities.Generators;

using System.Collections;

using SpaceBattle.Base;
using SpaceBattle.Collections;

class GameObjectsFuelGenerator : IEnumerator<int>
{
    private IList<object> objs;

    private IEnumerator<object> currentObjEnumerator;


    public GameObjectsFuelGenerator(IList<object> objs)
    {
        this.objs = objs;
        this.currentObjEnumerator = objs.GetEnumerator();
    }

    public int Current => Container.Resolve<int>("Game.Objects.Properties.Get", "Fuel", currentObjEnumerator.Current);

    object IEnumerator.Current => Current;

    public void Dispose() { }

    public bool MoveNext()
    {
        return this.currentObjEnumerator.MoveNext();
    }

    public void Reset()
    {
        this.currentObjEnumerator = this.objs.GetEnumerator();
    }
}
