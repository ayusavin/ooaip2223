namespace SpaceBattle.Base.Collections;

// Description:
//      Inversion of control container interface declaration
public interface IContainer
{
    ReturnType Resolve<ReturnType>(string key, params object[] argv);
}
