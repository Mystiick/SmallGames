using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownShooter.ECS.Components
{
    public class Bullet : Component
    {
        public int Damage { get; set; }
        public Entity Owner { get; set; }
        public Action<Entity, Entity> OnBulletHit { get; set; }
    }
}
