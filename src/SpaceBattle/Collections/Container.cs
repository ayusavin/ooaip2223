namespace SpaceBattle.Collections;

using SpaceBattle.Base;
using IContainer = Base.Collections.IContainer;

using DryIoc;

// Description:
//        Implementation of the IContainer interface, which is a wrapper over
//        the Inversion of Control container from the DryIoc microframework
public class Container : IContainer
{

    private readonly DryIoc.Container container;

    public Container()
    {
        this.container = new DryIoc.Container();

        IStrategy crs = new ContainerRegisterStrategy(this.container);
        this.container.RegisterInstance(crs, serviceKey: "IoC.Register");
    }

    public ReturnType Resolve<ReturnType>(string key, params object[] argv)
    {
        return (ReturnType)container.Resolve<IStrategy>(serviceKey: key).Run(argv);
    }

}

// Description:
//       Default Container dependency register strategy
public class ContainerRegisterStrategy : IStrategy
{
    private readonly DryIoc.Container container;

    public ContainerRegisterStrategy(DryIoc.Container cont)
    {
        container = cont;
    }

    public object Run(params object[] argv)
    {
        var ServiceKey = (string)argv[0];
        var Strategy = (Type)argv[1];
        this.container.Register(typeof(IStrategy), Strategy, serviceKey: ServiceKey);

        return 0;
    }
}
