namespace SpaceBattle.Entities.Strategies;

using SpaceBattle.Base;
using SpaceBattle.Collections;

public class ExceptionHandlerFindStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        ICommand command = (ICommand)argv[0];
        Exception exception = (Exception)argv[1];

        var ExceptionHandlers = Container.Resolve<IDictionary<ICommand, IDictionary<Exception, IStrategy>>>("Handler.Exception");

        return ExceptionHandlers[command][exception];
    }
}
