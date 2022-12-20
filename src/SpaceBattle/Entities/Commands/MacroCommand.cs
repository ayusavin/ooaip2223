namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;

// Description:
//      An implementation of a command pattern that takes a set of
//      commands and executes them in turn.
// Parametres:
//      Constructor takes next parametres:
//          IList<ICommand> commands:
//              A collection of commands that will 
//              be run in turn when the Run() method is called.
public class MacroCommand : ICommand
{
    IList<ICommand> cmds;

    public MacroCommand(IList<ICommand> commands) {
        this.cmds = commands;
    }

    public void Run()
    {
        foreach(ICommand cmd in cmds) {
            cmd.Run();
        }
    }
}
