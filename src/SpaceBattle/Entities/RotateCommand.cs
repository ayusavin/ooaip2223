namespace SpaceBattle.Entities;
using SpaceBattle.Base;

public class RotateCommand : ICommand
{

    private IRotatable Rotatable;

    public RotateCommand(IRotatable target)
    {
        this.Rotatable = target;
    }

    public void Run()
    {
        if (this.Rotatable.Rotation.Count != this.Rotatable.RotationSpeed.Count) {
            throw new ArgumentOutOfRangeException("vectors have different sizes");
        }

        for(int i = 0, count = this.Rotatable.Rotation.Count; i < count; i++) {
            this.Rotatable.Rotation[i] = this.Rotatable.Rotation[i].Add(this.Rotatable.RotationSpeed[i]);
        }
    }
}
