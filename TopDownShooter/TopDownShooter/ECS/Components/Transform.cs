using Microsoft.Xna.Framework;

namespace MystiickCore.ECS.Components;

public class Transform : Component
{
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; }
    public Vector2 TargetPosition { get; set; }

    public Transform()
    {
        Scale = Vector2.One;
    }
}
