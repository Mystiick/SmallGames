using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TopDownShooter.ECS;

namespace TopDownShooter.Intelligences
{
    public interface IImplementation
    {
        Entity CurrentEntity { get; set; }
        Entity PlayerEntity { get; set; }

        void Update(GameTime gameTime, List<Entity> allEntities);
    }
}
