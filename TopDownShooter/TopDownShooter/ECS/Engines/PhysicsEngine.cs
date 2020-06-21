using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using TopDownShooter.ECS.Components;

namespace TopDownShooter.ECS.Engines
{
    /// <summary>
    /// Engine that handles collisions and the Transform.Position update for an entity
    /// </summary>
    public class PhysicsEngine : Engine
    {
        public override Type[] RequiredComponents => new Type[] { typeof(Transform), typeof(BoxCollider) };

        public override void Update(GameTime gameTime, List<Entity> allEntities)
        {
            base.Update(gameTime, allEntities);

            for (int i = 0; i < this.Entities.Count; i++)
            {
                var x = this.Entities[i];
                // Look for any non zero Target positions, and make sure we can actually move to that position
                List<Entity> overlappingEntities = GetOverlappingEntities(x); 
               
                if (overlappingEntities.Count != 0)
                {
                    // If we are colliding with anything, alert the colliders
                    foreach (Entity e in overlappingEntities)
                    {
                        // Alert the collider that it has been collided
                        x.Collider.OnCollisionEnter?.Invoke(x, e);

                        // Alert the other collider something has hit it
                        e.Collider.OnCollisionHitMe?.Invoke(e, x);
                    }
                }

                if (x.Collider.Trigger)
                {
                    // Moving Triggers don't need to worry about collisions preventing movement
                    // Move it regardless, but the collision may cause it to be destroyed
                    x.Transform.Position = x.Transform.TargetPosition;
                }
                else
                {
                    // Try to move non trigger entities, if we can
                    HandleNonTriggerMovement(x, overlappingEntities);
                }
            }
        }

        private List<Entity> GetOverlappingEntities(Entity entity)
        {
            List<Entity> output = new List<Entity>();

            for (int i = 0; i < this.Entities.Count; i++)
            {
                var other = this.Entities[i];

                // Don't need to check this with itself
                if (entity.ID == other.ID) 
                {
                    continue;
                }

                // Normal collision check
                if (entity.Collider.TargetBoundingBox.Intersects(other.Collider.WorldBoundingBox))
                {
                    output.Add(other);
                }
                else if (entity.Collider.Continuous && entity.HasComponent<Velocity>())
                {
                    // Distance between the last location and the current location of the current entity
                    float distance = Vector2.Distance(entity.Collider.WorldBoundingBox.Center.ToVector2(), entity.Collider.TargetBoundingBox.Center.ToVector2());

                    // Shoot a ray from the last location towards the current location
                    var val = PhysicsEngine.CastRay(entity.Collider.WorldBoundingBox.Center.ToVector2(), entity.GetComponent<Velocity>().Direction, other.Collider);

                    // If there's a collision, and the collision is less than the distance between the old and new location, there was a raycast hit
                    if (val.HasValue && val.Value <= distance)
                    {
                        output.Add(other);
                    }
                }
            }

            return output.ToList();
        }

        private void HandleNonTriggerMovement(Entity x, IEnumerable<Entity> overlappingEntities)
        {
            // Make sure we aren't attempting to move into another non-trigger collider
            if (overlappingEntities.Count(y => !y.Collider.Trigger) == 0)
            {
                x.Transform.Position = x.Transform.TargetPosition;
            }
            else
            {
                Rectangle newTarget = x.Collider.WorldBoundingBox;
                Vector2 newPosition = x.Transform.Position;

                // Try moving X
                newTarget.X = x.Collider.TargetBoundingBox.X;
                if (!overlappingEntities.Any(y => y.Collider.WorldBoundingBox.Intersects(newTarget)))
                {
                    newPosition.X = x.Transform.TargetPosition.X;
                }
                else
                {
                    // There's still a collision, can't move X.
                    // Reset it so we can still check Y
                    newTarget.X = x.Collider.WorldBoundingBox.X;
                }

                // Try moving Y
                newTarget.Y = x.Collider.TargetBoundingBox.Y;
                if (!overlappingEntities.Any(y => y.Collider.WorldBoundingBox.Intersects(newTarget)))
                {
                    newPosition.Y = x.Transform.TargetPosition.Y;
                }

                x.Transform.Position = newPosition;
            }
        }

        /// <summary>
        /// Gets the intersection distance between <paramref name="r"/> and <paramref name="targetCollider"/>
        /// </summary>
        public static float? CastRay(Ray r, BoxCollider targetCollider)
        {
            BoundingBox bb = new BoundingBox(
                new Vector3(targetCollider.WorldBoundingBox.Location.ToVector2(), 0f),
                new Vector3((targetCollider.WorldBoundingBox.Location + targetCollider.WorldBoundingBox.Size).ToVector2(), 0f)
            );

            return r.Intersects(bb);
        }

        /// <summary>
        /// Casts a Ray from the <paramref name="origin"/> toward <paramref name="direction"/>. 
        /// If a collision with <paramref name="targetCollider"/> is found at any range, the output float? will have a value
        /// </summary>
        /// <param name="origin">World location to begin the ray</param>
        /// <param name="direction">Direction *relative to the origin* to shoot the ray</param>
        /// <param name="targetCollider">Collider to check</param>
        public static float? CastRay(Vector2 origin, Vector2 direction, BoxCollider targetCollider)
        {
            Ray r = new Ray(new Vector3(origin, 0f), new Vector3(direction, 0f));
            return CastRay(r, targetCollider);
        }

        /// <summary>
        /// Gets all colliding entities between <paramref name="origin"/> and <paramref name="direction"/>*<paramref name="maxDistance"/>
        /// </summary>
        public static Entity[] CastAll(Vector2 origin, Vector2 direction, float maxDistance, List<Entity> targets)
        {
            var output = new List<Entity>();
            Ray r = new Ray(new Vector3(origin, 0f), new Vector3(direction, 0f));

            for (int i = 0; i < targets.Count; i++)
            {
                Entity temp = targets[i];
                float? rayDistance;

                if (temp.Collider != null)
                {
                    rayDistance = CastRay(r, temp.Collider);

                    if (rayDistance.HasValue && rayDistance.Value < maxDistance)
                    {
                        output.Add(targets[i]);
                    }
                }
            }

            return output.ToArray();
        }

        #region | DEBUG |
#if DEBUG
        public override void Draw(SpriteBatch sb, GraphicsDevice gd, OrthographicCamera camera)
        {
            base.Draw(sb, gd, camera);
            if (Debugger.ShowDebugInfo)
            {
                this.Entities.ForEach(x =>
                {
                    DrawRectangle(x.Collider.WorldBoundingBox, gd, camera);
                    DrawRectangle(x.Collider.TargetBoundingBox, gd, camera);
                });
            }
        }

        public void DrawRectangle(Rectangle rect, GraphicsDevice gd, OrthographicCamera camera) 
        {
            Vector3 topLeft = new Vector3(rect.Left, rect.Top, 0);
            Vector3 topRight = new Vector3(rect.Right, rect.Top, 0);
            Vector3 bottomLeft = new Vector3(rect.Left, rect.Bottom, 0);
            Vector3 bottomRight = new Vector3(rect.Right, rect.Bottom, 0);

            var verts = new VertexPositionColor[] {
                new VertexPositionColor(topLeft, Color.White),
                new VertexPositionColor(topRight, Color.White),
                new VertexPositionColor(bottomRight, Color.White),
                new VertexPositionColor(bottomLeft, Color.White),
                new VertexPositionColor(topLeft, Color.White)
            };

            var basicEffect = new BasicEffect(gd);
            basicEffect.World = Matrix.CreateOrthographicOffCenter(
                camera.BoundingRectangle.Left,
                camera.BoundingRectangle.Right,
                camera.BoundingRectangle.Bottom,
                camera.BoundingRectangle.Top,
            0, 1);

            EffectTechnique effectTechnique = basicEffect.Techniques[0];
            EffectPassCollection effectPassCollection = effectTechnique.Passes;
            foreach (EffectPass pass in effectPassCollection)
            {
                pass.Apply();
                gd.DrawUserPrimitives(PrimitiveType.LineStrip, verts, 0, 4);
            }
        }
#endif
        #endregion

    }
}
