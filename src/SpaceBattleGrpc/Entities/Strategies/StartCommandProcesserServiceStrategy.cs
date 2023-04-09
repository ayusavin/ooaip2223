namespace SpaceBattleGrpc.Entities.Strategies;

using Microsoft.AspNetCore.Builder;
using SpaceBattle.Base;
using SpaceBattleGrpc.Services;

public class StartCommandProcesserServiceStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        var builderArgs = (string[])argv[0];

        return new StartCommandProcesserServiceCommand(args: builderArgs);
    }
}

class StartCommandProcesserServiceCommand : ICommand
{

    WebApplication app;

    public StartCommandProcesserServiceCommand(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddGrpc();

        this.app = builder.Build();

        app.MapGrpcService<CommandProcesserService>();
    }

    public void Run() => this.app.Run();
}
