namespace SpaceBattle.Collections;

using SpaceBattle.Base.Collections;
using SpaceBattle.Base;

using Hwdtech;
using Hwdtech.Ioc;

public class HWDTechContainer : IContainer
{
    static private readonly Dictionary<string, IStrategy> strategies = new Dictionary<string, IStrategy>();

    static HWDTechContainer() {
        strategies.Add("IoC.Register", new RegisterStrategy());
        strategies.Add("Scopes.Current.Set", new CurrentScopeSetStrategy());

        new InitScopeBasedIoCImplementationCommand().Execute();
    }

    public HWDTechContainer() {
        //this.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", this.Resolve<object>("Scopes.New", this.Resolve<object>("Scopes.Current"))).Execute();
    }
    public ReturnType Resolve<ReturnType>(string key, params object[] argv)
    {
        object result;
        try {
            result = strategies[key].Run(argv);
        } 
        catch(Exception) {
            result = IoC.Resolve<ReturnType>(key, argv)!;
        }
        return (ReturnType)result;
    }
}

class RegisterStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        string name = (string)argv[0];
        Type type = (Type)argv[1];

        IStrategy strategy = (IStrategy)Activator.CreateInstance(type)!;
        var Delegate = (object[] args) => {return strategy.Run(args);};

        var cmd = new HWDCommandAdapter(IoC.Resolve<Hwdtech.ICommand>("IoC.Register", name, Delegate));

        return cmd;
    }
}

class CurrentScopeSetStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        return new HWDCommandAdapter(IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", argv));
    }
}

class HWDCommandAdapter : SpaceBattle.Base.ICommand
{
    Hwdtech.ICommand cmd;
    public HWDCommandAdapter(Hwdtech.ICommand cmd) {
        this.cmd  = cmd;
    }

    public void Run()
    {
        cmd.Execute();
    }
}
