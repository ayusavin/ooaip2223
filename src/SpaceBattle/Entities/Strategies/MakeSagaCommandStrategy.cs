using SpaceBattle.Base;
using SpaceBattle.Entities.Commands;
using SpaceBattle.Collections;

namespace SpaceBattle.Entities.Strategies;

public class MakeSagaCommandStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        var commandNames = argv[0] as string[];
        var uObject = argv[1];

        if (commandNames == null)
            throw new ArgumentNullException(nameof(commandNames), "Command names array cannot be null");

        // Create transaction pairs from command names
        var transactionPairs = commandNames.Select(commandName =>
        {
            var forwardCommand = Container.Resolve<ICommand>(commandName, uObject);
            var backwardCommand = Container.Resolve<ICommand>("BackwardCommand", commandName, uObject);
            return new TransactionPair(forwardCommand, backwardCommand);
        }).ToList();

        return new SagaCommand(transactionPairs);
    }
}
