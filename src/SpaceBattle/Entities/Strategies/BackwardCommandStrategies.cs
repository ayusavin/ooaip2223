using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Commands;

namespace SpaceBattle.Entities.Strategies;

public abstract class BackwardCommandNameMixin
{
    protected static string GetBackwardCommandName(string commandName) => $"BackwardCommand.{commandName}";
}

public class RegisterBackwardCommandStrategy : BackwardCommandNameMixin, IStrategy
{
    public object Run(params object[] argv)
    {
        string forwardCommandName = (string)argv[0];
        ICommand backwardCommand = (ICommand)argv[1];

        string backwardCommandKey = GetBackwardCommandName(forwardCommandName);

        // Register the backward command in the Container
        Container.Resolve<ICommand>("IoC.Register", backwardCommandKey, (object[] _) => backwardCommand).Run();

        return new EmptyCommand();
    }
}


public class ResolveBackwardCommandStrategy : BackwardCommandNameMixin, IStrategy
{
    private readonly IStrategy onResolveError;
    private static readonly IStrategy defaultOnResolveError = new DelegateStrategy(_ => new EmptyCommand());

    public ResolveBackwardCommandStrategy(IStrategy? onResolveError)
    {
        this.onResolveError = onResolveError ?? defaultOnResolveError;
    }

    public object Run(params object[] argv)
    {
        string forwardCommandName = (string)argv[0];
        object[] additionalArgs = argv.Skip(1).ToArray();

        string backwardCommandKey = GetBackwardCommandName(forwardCommandName);

        try
        {
            // Attempt to resolve the backward command
            return Container.Resolve<ICommand>(backwardCommandKey, additionalArgs);
        }
        catch (Exception)
        {
            // If the backward command is not found, return an EmptyCommand
            return onResolveError.Run();
        }
    }
}
