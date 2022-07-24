using MystiickCore.ECS;

namespace MystiickCore.ECS.Components;

public class Intelligence : Component
{
    public BaseIntelligence Implementation { get; set; }
}
