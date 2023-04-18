namespace SpaceBattle.Base;

public interface IPullable<T>
{
    T Pull();

    bool Empty();
}
