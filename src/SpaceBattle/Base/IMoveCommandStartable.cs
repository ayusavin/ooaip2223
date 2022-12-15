namespace SpaceBattle.Base;
using SpaceBattle.Base.Collections;

public interface IMoveCommandStartable
{
    IUObject UObject { get; }

    IList<int> Velocity { get; }

    IQueue<ICommand> Queue { get; }
}
