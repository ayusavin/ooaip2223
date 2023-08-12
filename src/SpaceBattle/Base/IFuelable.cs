namespace SpaceBattle.Base;

public interface IFuelable
{
    int Level { get; set; }

    int BurnSpeed { get; }
}
