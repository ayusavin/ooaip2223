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
    public void SendCommand_CommandProcessed_WithoutArgs_Succesful()
    {
        // Init deps
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var cr = new CommandRequest
        {
            GameId = "42",
            Command = "Command.Mock"
        };

        ICommand streamBackend = null!;

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Workers.Stream.Push",
            (object[] argv) =>
            {
                string id = (string)argv[0];
                ICommand cmd = (ICommand)argv[1];

                var pushCmd = new Mock<ICommand>();

                pushCmd.Setup(c => c.Run()).Callback(
                    () =>
                    {
                        if (id != cr.GameId)
                            throw new Exception();
                        streamBackend = cmd;
                    }
                );
                return pushCmd.Object;
            }
        ).Run();

        ICommand testCmd = new Mock<ICommand>().Object;

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Commands.Generate.ByName",
            (object[] argv) =>
            {
                string name = (string)argv[0];

                if (name != cr.Command)
                    throw new Exception();

                return testCmd;
            }
        ).Run();

        var cps = new CommandProcesserService(new Mock<ILogger<CommandProcesserService>>().Object);

        cps.SendCommand(cr, new Mock<ServerCallContext>().Object);

        // Assertation
        Assert.Same(testCmd, streamBackend);
    }

    [Fact(Timeout = 1000)]
    public void SendCommand_CommandProcessed_WithArgs_Succesful()
    {
        // Init deps
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        var opts = new Dictionary<string, string> {
            {".NET", "Microsoft"},
            {"gRPC", "Google"},
            {"k8s", "Google"}
        };

        var cr = new CommandRequest
        {
            GameId = "42",
            Command = "Command.Mock"
        };
        cr.Argv.Add(
                opts.Select(Opt =>
                {
                    Option opt = new Option();
                    opt.Key = Opt.Key;
                    opt.Value = Opt.Value;

                    return opt;
                }
            )
        );

        ICommand streamBackend = null!;

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Workers.Stream.Push",
            (object[] argv) =>
            {
                string id = (string)argv[0];
                ICommand cmd = (ICommand)argv[1];

                var pushCmd = new Mock<ICommand>();

                pushCmd.Setup(c => c.Run()).Callback(
                    () =>
                    {
                        if (id != cr.GameId)
                            throw new Exception();
                        streamBackend = cmd;
                    }
                );
                return pushCmd.Object;
            }
        ).Run();

        ICommand testCmd = new Mock<ICommand>().Object;

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Commands.Generate.ByName",
            (object[] argv) =>
            {
                string name = (string)argv[0];
                var args = (IDictionary<string, string>)argv[1];

                if (name != cr.Command || !args.SequenceEqual(opts))
                    throw new Exception();

                return testCmd;
            }
        ).Run();

        var cps = new CommandProcesserService(new Mock<ILogger<CommandProcesserService>>().Object);

        cps.SendCommand(cr, new Mock<ServerCallContext>().Object);

        // Assertation
        Assert.Same(testCmd, streamBackend);
    }
}
