namespace SpaceBattleTests.Collections;
using SpaceBattle.Collections;
using SpaceBattle.Base;

internal class TestInjector : IStrategy
{
    public object Run(params object[] argv)
    {
        return new TestDependency();
    }
}

public class ContainerTests
{
    [Fact]
    void RegisterAndResolveTest()
    {
        var cont = new Container();

        string depName = "Test.Dependency";
        cont.Resolve<int>("IoC.Register", depName, typeof(TestInjector));

        IDependency svc = cont.Resolve<IDependency>(depName);
        Assert.True(svc is TestDependency);
    }
}
