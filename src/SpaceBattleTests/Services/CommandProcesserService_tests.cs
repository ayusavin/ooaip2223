namespace SpaceBattleTests.Services;

using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattleGrpc;
using SpaceBattleGrpc.Services;

public class CommandProcesserServiceTests
{
    [Fact(Timeout = 1000)]
    public async Task SendCommand_GameExists_Successful()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        // Setup location registry
        var registry = new Dictionary<string, string>
        {
            { "game1", "thread1" }
        };
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Game.Location.Registry",
            (object[] _) => registry
        ).Run();

        // Setup command generation
        var generatedCommand = new Mock<ICommand>();
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Commands.Generate.ByName",
            (object[] argv) => generatedCommand.Object
        ).Run();

        // Setup game command creation
        var gameCommand = new Mock<ICommand>();
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Commands.Game.Create",
            (object[] argv) => gameCommand.Object
        ).Run();

        // Setup stream push command
        var streamPushCommand = new Mock<ICommand>();
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Workers.Stream.Push",
            (object[] argv) =>
            {
                Assert.Equal("thread1", argv[0]);
                Assert.Same(gameCommand.Object, argv[1]);
                return streamPushCommand.Object;
            }
        ).Run();

        // Create service
        var logger = new Mock<ILogger<CommandProcesserService>>();
        var service = new CommandProcesserService(logger.Object);

        // Create request
        var request = new CommandRequest
        {
            GameId = "game1",
            Command = "TestCommand"
        };
        request.Argv.Add(new Option { Key = "arg1", Value = "value1" });

        // Send command
        var response = await service.SendCommand(request, new Mock<ServerCallContext>().Object);

        // Assert
        Assert.Equal(200, response.Status);
        streamPushCommand.Verify(c => c.Run(), Times.Once);
    }

    [Fact(Timeout = 1000)]
    public async Task SendCommand_GameNotFound_Returns404()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        // Setup empty location registry
        var registry = new Dictionary<string, string>();
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Game.Location.Registry",
            (object[] _) => registry
        ).Run();

        // Create service
        var logger = new Mock<ILogger<CommandProcesserService>>();
        var service = new CommandProcesserService(logger.Object);

        // Create request
        var request = new CommandRequest
        {
            GameId = "nonexistent",
            Command = "TestCommand"
        };

        // Send command
        var response = await service.SendCommand(request, new Mock<ServerCallContext>().Object);

        // Assert
        Assert.Equal(404, response.Status);
    }
}
