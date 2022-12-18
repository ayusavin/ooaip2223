namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;
using SpaceBattle.Collections;

public class CollisionCheckCommand : ICommand
{
    private IUObject first, second;

    public CollisionCheckCommand(IUObject first, IUObject second)
    {
        this.first = first;
        this.second = second;
    }

    public void Run()
    {
        var container = new Container();

        bool result = container.Resolve<bool>("Events.Collision.Determinant",
                                               this.first,
                                               this.second);

        if (result)
        {
            throw new Exception(string.Format("There was a conflict between {0} and {1}", first.ToString(), second.ToString()));
        }
    }
}
