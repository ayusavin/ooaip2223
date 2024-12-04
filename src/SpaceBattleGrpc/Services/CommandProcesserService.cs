namespace SpaceBattleGrpc.Services;

using Grpc.Core;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattleGrpc;

public class CommandProcesserService : CommandProcesser.CommandProcesserBase
{
    private readonly ILogger<CommandProcesserService> _logger;
    public CommandProcesserService(ILogger<CommandProcesserService> logger)
    {
        _logger = logger;
    }

    public override Task<CommandResponse> SendCommand(CommandRequest request, ServerCallContext context)
    {
        string gameId = request.GameId;
        string commandName = request.Command;

        IDictionary<string, string> argv = new Dictionary<string, string>();
        request.Argv.Select(arg => argv[arg.Key] = arg.Value).ToArray();

        // Get the thread ID where the game is currently running
        var locationRegistry = Container.Resolve<IDictionary<string, string>>("Game.Location.Registry");
        string threadId;

        if (!locationRegistry.TryGetValue(gameId, out threadId))
        {
            // If game location is not found, return error
            return Task.FromResult(new CommandResponse
            {
                Status = 404
            });
        }

        // Create the game command
        var gameCommand = Container.Resolve<ICommand>("Commands.Game.Create",
            Container.Resolve<ICommand>("Commands.Generate.ByName", commandName, argv),
            gameId
        );

        // Push the command to the correct thread's stream
        Container.Resolve<ICommand>(
            "Workers.Stream.Push",
            threadId,
            gameCommand
        ).Run();

        return Task.FromResult(new CommandResponse
        {
            Status = 200
        });
    }
}
