namespace SpaceBattle.Entities.Adapters;

using SpaceBattle.Base;

public class ReverseMoveAdapter : IMovable
{
    private readonly IMovable _movable;

    public ReverseMoveAdapter(IMovable movable)
    {
        _movable = movable;
    }

    public IList<int> Position
    {
        get => _movable.Position;
        set => _movable.Position = value;
    }

    public IList<int> MoveSpeed
    {
        get
        {
            return _movable.MoveSpeed.Select(x => -x).ToList();
        }
    }
}
