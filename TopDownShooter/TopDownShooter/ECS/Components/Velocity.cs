using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TopDownShooter.ECS.Components
{
    public class Velocity : Component
    {
        public float Speed { get; set; }
        public Vector2 Direction { get; set; }
    }
}
