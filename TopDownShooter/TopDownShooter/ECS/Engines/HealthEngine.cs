using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TopDownShooter.ECS.Components;

namespace TopDownShooter.ECS.Engines
{
    public class HealthEngine : Engine
    {
        public override Type[] RequiredComponents => new[] { typeof(Health) };

        public override void Update(GameTime gameTime, List<Entity> allEntities)
        {
            for (int i = 0; i < this.Entities.Count; i++)
            {
                var x = this.Entities[i];
                var h = x.GetComponent<Health>();

                if (h.CurrentHealth <= 0)
                {
                    x.Expired = true;
                }
            }
        }
    }
}
