using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TopDownShooter.ECS;
using TopDownShooter.ECS.Components;

namespace TopDownShooter.Intelligences
{
    public class Dummy : BaseIntelligence
    {
        public override void Update(GameTime gameTime, List<Entity> allEntities)
        {
            CurrentEntity.Transform.Rotation += .1f;
        }
    }
}
