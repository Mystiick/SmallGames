using Microsoft.Xna.Framework;

namespace MystiickCore.ECS.Components
{
    public class Velocity : Component
    {
        public float Speed { get; set; }
        public Vector2 Direction { get; set; }
    }
}
