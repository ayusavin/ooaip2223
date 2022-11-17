namespace SpaceBattle.Base;

public interface IRotatable
{
    IList<IAngle> Rotation { get; set; }

    IList<IAngle> RotationSpeed { get; }
}