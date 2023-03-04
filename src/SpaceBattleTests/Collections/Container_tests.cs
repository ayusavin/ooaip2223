namespace SpaceBattleTests.Collections;

using SpaceBattle.Base;
using SpaceBattle.Collections;

public class ContainerTests
{

    [Fact]
    void ContainerResolveRegister_Succesful()
    {
        Container.Resolve<ICommand>("Scopes.Current.Set", Container.Resolve<object>("Scopes.New", Container.Resolve<object>("Scopes.Root"))).Run();
        Container.Resolve<ICommand>("IoC.Register", "Test.Dependency", typeof(TestInjector)).Run();

        var dep = Container.Resolve<IDependency>("Test.Dependency");

        Assert.IsType(typeof(TestDependency), dep);
    }

}

internal class TestInjector : IStrategy
{
    public object Run(params object[] argv)
    {
        return new TestDependency();
    }
}
