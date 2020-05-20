﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using TopDownShooter.ECS.Components;

namespace TopDownShooter.ECS.Engines
{
    /// <summary>
    /// Engine that handles moving an entity's TargetBoundingBox to enable movement
    /// </summary>
    public class TransformEngine : Engine
    {
        public override Type[] RequiredComponents => new Type[] { typeof(Transform) }; // , typeof(Velocity) optional

        public TransformEngine()
        {
            
        }

        public override void Update(GameTime gameTime, List<Entity> allEntities)
        {
            base.Update(gameTime, allEntities);

            for (int i = 0; i < this.Entities.Count; i++)
            {
                var x = this.Entities[i];
                var v = x.GetComponent<Velocity>();

                if (v != null)
                {
                    var newDirection = v.Direction.NormalizedCopy();
                    if (!float.IsNaN(newDirection.X) && !float.IsNaN(newDirection.Y))
                    {
                        v.Direction = newDirection;
                    }

                    x.Transform.TargetPosition = x.Transform.Position + (v.Direction * v.Speed * (int)gameTime.ElapsedGameTime.TotalMilliseconds);
                }
                else
                {
                    x.Transform.TargetPosition = x.Transform.Position + Vector2.Zero * 0 * (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                }

                if (x.Collider != null)
                {
                    // Need to do it this way as x.Collider.TargetBoundingBox is a struct, and must be assigned to directly, instead of modified
                    var targetBoundingBox = x.Collider.BoundingBox;
                    targetBoundingBox.X += (int)x.Transform.TargetPosition.X;
                    targetBoundingBox.Y += (int)x.Transform.TargetPosition.Y;
                    x.Collider.TargetBoundingBox = targetBoundingBox;

                    // Need to do it this way as x.Collider.WorldBoundingBox is a struct, and must be assigned to directly, instead of modified
                    var worldBoundingBox = x.Collider.BoundingBox;
                    worldBoundingBox.X += (int)x.Transform.Position.X;
                    worldBoundingBox.Y += (int)x.Transform.Position.Y;
                    x.Collider.WorldBoundingBox = worldBoundingBox;
                }
            }
        }
    }
}
