namespace SpaceBattle.Entities.Generators;

using System.Collections;
using SpaceBattle.Base;
using SpaceBattle.Base.Collections;

using SpaceBattle.Collections;

public class LinearPositionGenerator : IEnumerator<IList<int>>
{
    private int count;
    private int initialCount;

    private IList<int> initialPos;
    private IList<int> value;
    private IList<int> step;

    public LinearPositionGenerator(int count, IList<int> start, IList<int> step)
    {
        this.initialPos = this.value = start;
        this.initialCount = this.count = count;
        this.step = step;
    }

    public IList<int> Current => value;

    object IEnumerator.Current => this.Current;

    public void Dispose() { }

    public bool MoveNext()
    {
        if (count <= 0) return false;
        this.value = Container.Resolve<IList<int>>("Math.IList.Int32.Addition", this.value, this.step);

        --count;
        return true;
    }

    public void Reset()
    {
        this.count = this.initialCount;
        this.value = this.initialPos;
    }
}
