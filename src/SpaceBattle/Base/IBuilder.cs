namespace SpaceBattle.Base;

public interface IBuilder
{
    IBuilder Config(string param, params object[] argv);

    object GetOrCreate();
}
