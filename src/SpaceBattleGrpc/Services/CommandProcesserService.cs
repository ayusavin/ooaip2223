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
        string GameId = request.GameId;
        string CommandName = request.Command;

        IDictionary<string, string> argv = new Dictionary<string, string>();
        request.Argv.Select(arg => argv[arg.Key] = arg.Value).ToArray();

        Container.Resolve<ICommand>(
            "Workers.Stream.Push",
            GameId,
            Container.Resolve<ICommand>("Commands.Generate.ByName", CommandName, argv)
        ).Run();

        return Task.FromResult(new CommandResponse
        {
            Status = 200
        });
    }
}
