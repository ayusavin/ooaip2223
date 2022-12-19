namespace SpaceBattleTests.Entities.Commands;

using SpaceBattle.Base;
using SpaceBattle.Entities.Commands;

using Moq;

public class MacroCommandTests {

    [Theory(Timeout = 1000)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    public void MacroCommandRun_Successful(int numberOfCommands) {
        // Init dependencies
        IList<ICommand> cmds = new List<ICommand>();
        IList<Mock<ICommand>> mocks = new List<Mock<ICommand>>();
        
        System.Linq.Expressions.Expression<System.Action<ICommand>> Callback = mc => mc.Run();;

        for(int i = 0; i < numberOfCommands; ++i) {
            var MockCommand = new Mock<ICommand>();
            MockCommand.Setup(Callback).Verifiable();
            
            cmds.Add(MockCommand.Object);
            mocks.Add(MockCommand);
        }

        MacroCommand mc = new MacroCommand(cmds);

        // Action
        mc.Run();

        // Assertation
        foreach(var mock in mocks) {
            mock.Verify(Callback);
        }
    }

    [Fact(Timeout = 1000)]
    public void MacroCommandRun_CommandsListIsNull_ThrowsException() {
        // Init dependencies
        MacroCommand mc = new MacroCommand((IList<ICommand>)null!);

        // Assertation
        Assert.ThrowsAny<Exception>(() => mc.Run());
    }

    [Fact(Timeout = 1000)]
    public void MacroCommandRun_ExistNullCommand_ThrowsException() {
        // Init dependencies
        IList<ICommand> cmds = new List<ICommand>{null!};

        MacroCommand mc = new MacroCommand(cmds);

        // Assertation
        Assert.ThrowsAny<Exception>(() => mc.Run());
    }
}
