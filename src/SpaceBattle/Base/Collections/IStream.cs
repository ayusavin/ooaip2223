namespace SpaceBattle.Base.Collections;

public interface IStream<T>
{
    IPullable<T> Pullable { get; }

    IPushable<T> Pushable { get; }
}
