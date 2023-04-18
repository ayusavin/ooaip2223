namespace SpaceBattle.Base;


public interface IWorker
{

    IStrategy Behaviour { get; set; }

    void Start();

    void Stop();

}
