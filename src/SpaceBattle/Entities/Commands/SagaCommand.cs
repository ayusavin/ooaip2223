using SpaceBattle.Base;

namespace SpaceBattle.Entities.Commands;

// Type declaration for transaction pair
public record TransactionPair(ICommand ForwardCommand, ICommand BackwardCommand);

public class SagaCommand : ICommand
{
    private readonly IEnumerable<TransactionPair> transactionPairs;

    public SagaCommand(
        IEnumerable<TransactionPair> transactionPairs
    )
    {
        this.transactionPairs = transactionPairs;
    }

    public void Run()
    {
        var redo = () => { };

        try
        {
            foreach (var (forward, backward) in transactionPairs)
            {
                forward.Run();
                var oldRedo = redo;
                redo = () =>
                {
                    backward.Run();
                    oldRedo();
                };
            }
        }
        catch (Exception originalException)
        {
            try
            {
                redo();
                throw; // Re-throw the original exception if compensation succeeds
            }
            catch (Exception compensationException)
            {
                // If backward command fails, throw a new exception that includes both errors
                throw new AggregateException(originalException, compensationException);
            }
        }
    }
}
