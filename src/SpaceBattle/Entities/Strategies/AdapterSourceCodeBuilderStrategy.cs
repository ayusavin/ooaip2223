namespace SpaceBattle.Entities.Strategies;


using System.Linq;
using System.Reflection;
using SpaceBattle.Base;
using SpaceBattle.Collections;

public class AdapterSourceCodeBuilderStrategy : IStrategy
{
    public object Run(params object[] argv)
    {
        var dtype = (Type)argv[0];
        var builder = Container.Resolve<IBuilder>("Entities.Builders.AdapterSourceCode").Config("Dtype", dtype);

        foreach (PropertyInfo prop in dtype.GetProperties())
        {
            builder.Config("Property", prop);
        }

        var methodsWithoutAccessors = dtype.GetMethods().Except(
            dtype.GetProperties().Select((PropertyInfo p) => p.GetAccessors()).SelectMany((MethodInfo[] m) => m)
        );

        foreach (MethodInfo method in methodsWithoutAccessors)
        {
            builder.Config("Method", method);
        }

        return builder.GetOrCreate();
    }
}
