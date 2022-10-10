namespace SpaceBattle.Entities;
using SpaceBattle.Base;

public class MoveCommand : ICommand
{

    private IMovable movable;

    public MoveCommand(IMovable target)
    {
        this.movable = target;
    }

    public void Run()
    {
        if (movable == null || movable.Position == null || movable.MoveSpeed == null ||
         movable.Position.Count != movable.MoveSpeed.Count) // Different vector size check
        {
            throw new Exception("wrong vectors data");
        }

        for (int i = 0, Size = movable.Position.Count; i < Size; ++i)
        {
            movable.Position[i] += movable.MoveSpeed[i];
        }
    }
}