namespace SpaceBattle.Base;
using System.Numerics;

public interface IAngle
{
    // Description:
    //      The numerator and denominator represent the angle 
    //      of rotation as the aspect ratio of the legs 
    int Numerator {get; set;}

    int Denominator {get; set;}

    // Description:
    //      The Add operation adds two corners, thereby 
    //      turning one corner by the value of the other
    IAngle Add(IAngle other);
}