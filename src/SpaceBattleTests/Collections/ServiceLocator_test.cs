namespace SpaceBattleTests.Collections;
using SpaceBattle.Collections;
using SpaceBattle.Base;

internal class TestStatefulInjector : IStrategy
{
    IDependency dpd;

    public TestStatefulInjector(IDependency dpd)
    {
        this.dpd = dpd;
    }

    public object Run(params object[] argv)
    {
        return this.dpd;
    }
}

public class ServiceLocatorTessts
{


    [Fact]
    void RegisterAndLocateTest()
    {
        string depName = "Test.Dependency";
        var dependency = new TestDependency();

        ServiceLocator.Register(depName, new TestStatefulInjector(dependency));
        IDependency svc = ServiceLocator.Locate<IDependency>(depName);

        Assert.Same(svc, dependency);
    }
}
