namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;
using SpaceBattle.Collections;

// Description:
//      Implementation of a command that checks two objects for a collision.
//      When running the Run() method, checks if there was a collision between the first and second object.
// Parametres:
//      Constructor takes next parametres:
//          IUObject first:
//              The first object to check for collision with the second.
//          IUObject second:
//              The second object to check for collision with the first.
// Container dependencies:
//      bool Events.Collision.Determinant (IUObject, IUObject):
//          Strategy to detect if there is a collision between two IUObjects.
//
//          Returns:
//              true:
//                  A collision has occured.
//              false:
//                  There was no collision between these two objects.
// Exceptions:
//      Exception:
//          A collision has occured.
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
