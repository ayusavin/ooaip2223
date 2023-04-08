namespace SpaceBattle.Collections;

using Hwdtech;
using Hwdtech.Ioc;
using SpaceBattle.Base;

// Description:
//      Implementation of ineversion of control container,
//      uses IoC by HWDTech
public static class Container
{
    static private readonly Dictionary<string, IStrategy> strategies = new Dictionary<string, IStrategy>();

    // Description:
    //      Static constructor init internal rewrited dependencies and
    //      basis Hwdtech.IoC dependencies.
    static Container()
    {
        strategies.Add("IoC.Register", new RegisterStrategy());
        strategies.Add("Scopes.Current.Set", new CurrentScopeSetStrategy());
        strategies.Add("Scopes.Current", new CurrentScopeStrategy());

        new InitScopeBasedIoCImplementationCommand().Execute();
    }

    // Description:
    //      Resolve method returns the ReturnType dependency,
    //      with the name key and given arguments argv.
    public static ReturnType Resolve<ReturnType>(string key, params object[] argv)
    {
        object result;
        try
        {
            result = strategies[key].Run(argv);
        }
        catch (Exception)
        {
            result = IoC.Resolve<ReturnType>(key, argv)!;
        }
        return (ReturnType)result;
    }
}

// Description:
//      Strategy for registering new dependencies. Adapts strategies for delegates.
// Parametres:
//      Run method takes next parametres:
//          string argv[0]:
//              The name of the dependency key to register.
//          Type argv[1]:
//              The type of dependency being logged. The type must inherit the
//              IStrategy interface, otherwise it will throw an exception when casting to IStrategy.
// Returns:
//      ICommand:
//          A command that, when the Run() method is executed, will register the given dependency.
class RegisterStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        string name = (string)argv[0];

        Func<object[], object> Delegate;

        try
        {
            Type type = (Type)argv[1];
            IStrategy strategy = (IStrategy)Activator.CreateInstance(type)!;

            Delegate = (object[] args) => { return strategy.Run(args); };
        }
        catch (InvalidCastException)
        {
            Delegate = (Func<object[], object>)argv[1];
        }

        var cmd = new HWDCommandAdapter(IoC.Resolve<Hwdtech.ICommand>("IoC.Register", name, Delegate));

        return cmd;
    }
}

// Description:
//      A strategy that casts the "Scopes.Current.Set" strategy from the Hwdtech.ICommand
//      type to the SpaceBattle.Base.ICommand type.
class CurrentScopeSetStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        return new HWDCommandAdapter(IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", argv));
    }
}

class CurrentScopeStrategy : IStrategy
{
    public object Run(params object[] argv) { return IoC.Resolve<Func<object>>("Scopes.Current", argv)(); }
}

// Description:
//      Wrapper over Hwdtech.ICommand, casting it to the SpaceBattle.Base.ICommand type.
class HWDCommandAdapter : SpaceBattle.Base.ICommand
{
    Hwdtech.ICommand cmd;
    public HWDCommandAdapter(Hwdtech.ICommand cmd)
    {
        this.cmd = cmd;
    }

    public void Run()
    {
        cmd.Execute();
    }
}
