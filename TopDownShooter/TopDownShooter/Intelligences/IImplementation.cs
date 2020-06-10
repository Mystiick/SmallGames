using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TopDownShooter.ECS;
using TopDownShooter.ECS.Components;
using TopDownShooter.ECS.Engines;

namespace TopDownShooter.Intelligences
{
    public interface IImplementation
    {
        Entity CurrentEntity { get; set; }
        Entity PlayerEntity { get; set; }
        TileGrid Grid { get; set; }

        void Update(GameTime gameTime, List<Entity> allEntities);
    }
}
