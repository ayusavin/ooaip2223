namespace SpaceBattleTests.Entities.Commands;

using System;
using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;


public class WorkerCloseCommandTests
{

    [Fact(Timeout = 1000)]
    public void WorkerCloseCommand_WithoutCallback_Succesful()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        string id = "42";

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Workers.Stream.Close",
            (object[] argv) =>
            {
                if ((string)argv[0] != id) throw new Exception();
                return new Mock<ICommand>().Object;
            }
        ).Run();
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Workers.Registry.Get",
            (object[] _) => new Mock<IWorker>().Object
        ).Run();

        var wcc = new WorkerCloseCommand(id: id);

        // Assertation
        wcc.Run();
    }

    [Fact(Timeout = 1000)]
    public void WorkerCloseCommand_WithCallback_Succesful()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        string id = "42";

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Workers.Stream.Close",
            (object[] argv) =>
            {
                if ((string)argv[0] != id) throw new Exception();
                return new Mock<ICommand>().Object;
            }
        ).Run();
        Container.Resolve<ICommand>(
            "IoC.Register",
            "Workers.Registry.Get",
            (object[] _) => new Mock<IWorker>().Object
        ).Run();

        var wcc = new WorkerCloseCommand(id: id, action: new Mock<ICommand>().Object);

        // Assertation
        wcc.Run();
    }
}
