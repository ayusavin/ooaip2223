namespace SpaceBattle.Base.Collections;

public interface IQueue<T>
{
    void Push(T elem);

    T Pop();
}