namespace SpaceBattleTests.Collections;

using SpaceBattle.Collections;
using SpaceBattle.Base;

public class ContainerTests {
    
    [Fact]
    void ContainerResolveRegister_Succesful() {
        var container = new Container();
        
        container.Resolve<ICommand>("Scopes.Current.Set", container.Resolve<object>("Scopes.New", container.Resolve<object>("Scopes.Root"))).Run();
        container.Resolve<ICommand>("IoC.Register", "Test.Dependency", typeof(TestInjector)).Run();

        var dep = container.Resolve<IDependency>("Test.Dependency");

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