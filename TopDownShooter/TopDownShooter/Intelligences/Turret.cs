using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TopDownShooter.ECS;
using TopDownShooter.ECS.Components;

namespace TopDownShooter.Intelligences
{
    public class Turret : IImplementation
    {
        public Entity CurrentEntity { get; set; }
        public Entity PlayerEntity { get; set; }
        public TileGrid Grid { get; set; }

        public void Update(GameTime gameTime, List<Entity> allEntities)
        {
            if (CurrentEntity != null && PlayerEntity != null)
            {
                CurrentEntity.Transform.Rotation = Helpers.DetermineAngle(CurrentEntity.Transform.Position, PlayerEntity.Transform.Position);
            }
        }
    }
}
