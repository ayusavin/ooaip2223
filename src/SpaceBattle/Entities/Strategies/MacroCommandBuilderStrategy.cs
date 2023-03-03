namespace SpaceBattle.Entities.Strategies;

using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;

// Description:
//      Implementation of a strategy for building a macrocommand,
//      based on a list of dependencies and an object target.
// Parameters:
//      Run method required two parametres in argv:
//          string argv[0]:
//              IoC container key, that contains IList<string>,
//              collection of commands on the basis of which the macrocommand
//              will be built.
//
//          IUObject argv[1]:
//              An object that will be the target for executing
//              commands received from IoC by the key argv[0].
// Returns:
//      ICommand:
//          Created macrocommand object.
public class MacroCommandBuilderStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        string dependenciesListKey = (string)argv[0];
        IUObject obj = (IUObject)argv[1];

        IList<string> dependencies = Container.Resolve<IList<string>>(dependenciesListKey);

        IList<ICommand> commands = new List<ICommand>();

        foreach(string depName in dependencies) {
            commands.Add(Container.Resolve<ICommand>(depName, obj));
        }

        return new MacroCommand(commands);
    }
}
