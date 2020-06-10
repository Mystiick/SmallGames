using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TopDownShooter.ECS;

namespace TopDownShooter.Intelligences
{
    public class Turret : IImplementation
    {
        public Entity CurrentEntity { get; set; }
        public Entity PlayerEntity { get; set; }

        public void Update(GameTime gameTime, List<Entity> allEntities)
        {
            if (CurrentEntity != null && PlayerEntity != null)
            {
                CurrentEntity.Transform.Rotation = Helpers.DetermineAngle(CurrentEntity.Transform.Position, PlayerEntity.Transform.Position);
            }
        }
    }
}
