namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;

public class RestoreFuelCommand : ICommand
{
    private readonly IFuelable fuelable;
    private readonly int amount;

    public RestoreFuelCommand(IFuelable fuelable, int amount)
    {
        this.fuelable = fuelable;
        this.amount = amount;
    }

    public void Run()
    {
        fuelable.Level += amount;
    }
}
