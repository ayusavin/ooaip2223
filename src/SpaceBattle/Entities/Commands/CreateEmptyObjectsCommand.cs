namespace SpaceBattle.Entities.Commands;

using SpaceBattle.Base;
using SpaceBattle.Collections;

public class CreateEmptyObjectsCommand : ICommand
{
    private int objects_n;

    public CreateEmptyObjectsCommand(int objects_n)
    {
        this.objects_n = objects_n;
    }

    public void Run()
    {
        foreach (int _ in Enumerable.Range(0, objects_n))
        {
            Container.Resolve<ICommand>(
                "Game.Objects.Registry.Add",
                Container.Resolve<string>("System.Generator.Uuid"),
                Container.Resolve<object>("Game.Objects.Empty")
            ).Run();
        }
    }
}
