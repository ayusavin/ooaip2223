namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;
using SpaceBattle.Collections;

public class ConsumeFuelMoveCommand : ICommand
{
    private readonly ICommand saga;
    public ConsumeFuelMoveCommand(IUObject uObject)
    {
        saga = Container.Resolve<ICommand>(
            "MakeSagaCommand",
            new[] { "Move", "ConsumeFuel" },
            uObject
        );
    }

    public void Run()
    {
        saga.Run();
    }
}