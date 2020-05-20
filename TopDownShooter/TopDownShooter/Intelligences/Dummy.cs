using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TopDownShooter.ECS;


namespace TopDownShooter.Intelligences
{
    public class Dummy : IImplementation
    {
        public Entity CurrentEntity { get; set; }
        public Entity PlayerEntity { get; set; }

        public void Update(GameTime gameTime)
        {
            CurrentEntity.Transform.Rotation += .1f;
        }
    }
}
