namespace MystiickCore.ECS.Components;

public class Health : Component
{
    private int _maxHealth;

    public int CurrentHealth { get; set; }
    public int MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            var diff = value - _maxHealth;

            _maxHealth += diff;
            this.CurrentHealth += diff;
        }
    }

    public Health()
    {
    }

    public Health(int max)
    {
        this.MaxHealth = max;
    }
}
