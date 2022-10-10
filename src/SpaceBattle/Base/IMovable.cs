namespace SpaceBattle.Base;
using System.Collections.Generic;

// IMovable interface declare linear move objects
// properties to manipulate them
public interface IMovable
{
    // Description:
    //      Represents IMovable object position vector
    IList<int> Position { get; set; }

    // Description:
    //      Represents IMovable object move vector
    IList<int> MoveSpeed { get; }
}