namespace SpaceBattleTests.Misc.Collections;
using SpaceBattle.Base.Collections;
using SpaceBattle.Base;

// Use only for test purpouses
// Thread non-safe
public class ReRegisterableIoC : IContainer
{
    Dictionary<string, Type> registry = new Dictionary<string, Type>();
    public ReturnType Resolve<ReturnType>(string key, params object[] argv)
    {
        if (key is "IoC.Register")
        {
            var newKey = (string)argv[0];
            var type = (Type)argv[1];

            registry[newKey] = type;

            return default(ReturnType);
        }

        return (ReturnType)((IStrategy)Activator.CreateInstance(registry[key])!).Run(argv);
    }
}
