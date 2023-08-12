namespace SpaceBattle.Entities.Strategies;

using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Moq;
using SpaceBattle.Base;
using SpaceBattle.Collections;
using SpaceBattle.Entities.Builders;
using SpaceBattle.Entities.Strategies;

public class AdapterSourceCodeBuilderStrategyTests
{
    [Fact(Timeout = 1000)]
    void AdapterSourceCodeBuilderStrategy_producesValidSourceCode()
    {
        // Init test dependencies
        Container.Resolve<ICommand>(
            "Scopes.Current.Set",
            Container.Resolve<object>(
                "Scopes.New", Container.Resolve<object>("Scopes.Root")
            )
        ).Run();

        Container.Resolve<ICommand>(
            "IoC.Register",
            "Entities.Builders.AdapterSourceCode",
            (object[] _) =>
            {
                return new AdapterSourceCodeBuilder();
            }
        ).Run();

        var ascbs = new AdapterSourceCodeBuilderStrategy();

        // Action
        var template = (string)ascbs.Run(typeof(TestInterface));

        var compOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithUsings("System", "SpaceBattle.Base", "SpaceBattle.Entities.Strategies", "System.Collections.Generic");

        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { CSharpSyntaxTree.ParseText(template) },
            references: new[] {
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(TestInterface).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ICommand).GetTypeInfo().Assembly.Location)
            },
            options: compOptions
        );

        // Assertation
        Assert.Empty(compilation.GetDiagnostics()); // No compilation errors
        Assert.True(
            CSharpSyntaxTree.ParseText(template).IsEquivalentTo(
                CSharpSyntaxTree.ParseText(example)
            )
        );
    }

    string example = @"
internal class TestInterface_adapter : SpaceBattle.Entities.Strategies.TestInterface {
    private System.Collections.Generic.IDictionary<string, object> data;
    public TestInterface_adapter(System.Collections.Generic.IDictionary<string, object> data) {
        this.data = data;
    }
    public System.Int32 PropertyRW {
        get => (System.Int32)data[""System.Int32.PropertyRW""];set => data[""System.Int32.PropertyRW""] = value;
    }
    public System.String PropertyR {
        get => (System.String)data[""System.String.PropertyR""];
    }
    public System.Object PropertyW {
        set => data[""System.Object.PropertyW""] = value;
    }
    public void MethodVoid (System.Int32 arg1, System.Object arg2, SpaceBattle.Base.ICommand cmd) {
        ((System.Action<System.Int32,System.Object,SpaceBattle.Base.ICommand>)data[""void.MethodVoid""])(arg1, arg2, cmd);
    }
    public System.Int32 MethodInt () {
        return ((System.Func<System.Int32>)data[""System.Int32.MethodInt""])();
    }
}";

}

public interface TestInterface
{
    void MethodVoid(int arg1, object arg2, ICommand cmd);

    int MethodInt();

    int PropertyRW { get; set; }

    string PropertyR { get; }

    object PropertyW { set; }
}
