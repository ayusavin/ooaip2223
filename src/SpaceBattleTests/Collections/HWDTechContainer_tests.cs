namespace SpaceBattleTests.Collections;

using SpaceBattle.Collections;
using SpaceBattle.Base;

public class HWDTechContainerTests {
    
    [Fact]
    void ContainerResolveRegister_Succesful() {
        var container = new HWDTechContainer();
        
        container.Resolve<ICommand>("Scopes.Current.Set", container.Resolve<object>("Scopes.New", container.Resolve<object>("Scopes.Root"))).Run();
        container.Resolve<ICommand>("IoC.Register", "Test.Dependency", typeof(TestInjector)).Run();

        var dep = container.Resolve<IDependency>("Test.Dependency");

        Assert.IsType(typeof(TestDependency), dep);
    }

}
