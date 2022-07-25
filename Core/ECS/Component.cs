namespace MystiickCore.ECS;

public abstract class Component
{
    private Guid _id = Guid.NewGuid();

    public Guid ID
    {
        get => _id;
        private set => _id = value;
    }
}
