namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;

public class ConsumeFuelCommand : ICommand
{
    private readonly IFuelable fuelable;

    public ConsumeFuelCommand(IFuelable fuelable)
    {
        this.fuelable = fuelable;
    }

    public void Run()
    {
        if (fuelable.BurnSpeed <= 0)
            throw new InvalidOperationException("Burn speed must be positive");

        if (fuelable.Level < fuelable.BurnSpeed)
            throw new InvalidOperationException("Not enough fuel");

        fuelable.Level -= fuelable.BurnSpeed;
    }
}
