using System.Collections.Generic;
using Microsoft.Xna.Framework;

using MystiickCore.ECS;

namespace TopDownShooter.Intelligences
{
    public class Dummy : ShooterIntelligence
    {
        public override void Update(GameTime gameTime, List<Entity> allEntities)
        {
            CurrentEntity.Transform.Rotation += .1f;
        }
    }
}
