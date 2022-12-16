namespace SpaceBattle.Base;


// Description:
//     Declare of the strategy behaviour pattern interface
public interface IStrategy
{
    object Run(params object[] argv);
}