namespace SpaceBattleTests.Entities.Strategies;

using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Strategies;

using Moq;

public class MacroCommandBuilderStrategyTests {

    private string IoCDependencyKey = "Tests.Dependencies.Key";

    [Fact(Timeout = 1000)]
    public void MacroCommandBuilderRun_Successful() {
        // Init dependencies
        var container = new Container();

        container.Resolve<ICommand>(
            "Scopes.Current.Set", 
            container.Resolve<object>(
                "Scopes.New", container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        // Create dependencies names list
        IList<string> deps = (IList<string>)new DependenciesListInjector().Run();
        foreach(string depName in deps){
            container.Resolve<ICommand>("IoC.Register", depName, typeof(MacroCommandBuilderTestDependency)).Run();
        }
        
        var DependenciesListInjector = new Mock<IStrategy>();
        DependenciesListInjector.Setup(dli => dli.Run()).Returns(deps);

        container.Resolve<ICommand>("IoC.Register", IoCDependencyKey, typeof(DependenciesListInjector)).Run();

        var IUobjectMock = new Mock<IUObject>();

        var mcb = new MacroCommandBuilderStrategy();

        // Build macrocommand
        ICommand mc = (ICommand)mcb.Run(IoCDependencyKey, IUobjectMock.Object);

        // Run builded macrocommand
        mc.Run();

        // Assert
        Assert.True(MacroCommandBuilderTestDependency.WasExecuted);
    }
}

class MacroCommandBuilderTestDependency : IStrategy
{
    static public bool WasExecuted = false;

    public object Run(params object[] argv){
        var mockDep = new Mock<ICommand>();
        mockDep.Setup(mc => mc.Run()).Callback(() => WasExecuted = true);
        return mockDep.Object;
    }
}

class DependenciesListInjector : IStrategy
{
    public object Run(params object[] argv)
    {
        return new List<string>{
            "MacroCommandBuilderTestDependency"
        };
    }
}
