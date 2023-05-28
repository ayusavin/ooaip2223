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
        Assert.Empty(compilation.GetDiagnostics());
    }
}

public interface TestInterface
{
    void MethodVoid(int arg1, object arg2, ICommand cmd);

    int MethodInt();

    int PropertyRW { get; set; }

    string PropertyR { get; }

    object PropertyW { set; }
}
