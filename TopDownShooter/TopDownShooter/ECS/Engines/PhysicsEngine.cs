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
        private const int MAX_RESOLUTION_ATTEMPTS = 10;
        public override Type[] RequiredComponents => new Type[] { typeof(Transform), typeof(BoxCollider) };

        /// <summary>At this point, all of the entities must have their target positions and bounding boxes set. This engine will update the final positions based on the targets</summary>
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

        public List<Entity> GetOverlappingEntities(Entity entity)
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

        private void HandleNonTriggerMovement(Entity me, IEnumerable<Entity> overlappingEntities)
        {
            // If a collider is static, it isn't ever going to move, no need to handle movement, something just ran into me
            if (!me.Collider.Static)
            {
                // Only try MAX_RESOLUTION_ATTEMPTS so we don't get stuck in an infinte loop resolving collisions
                for (int i = 0; i < MAX_RESOLUTION_ATTEMPTS; i++)
                {
                    // Resolve the current collision, and make sure the entity isn't colliding with another entity now
                    me.Transform.TargetPosition = ResolveCollisions(me, overlappingEntities.Where(x => !x.Collider.Trigger).ToList());
                    overlappingEntities = GetOverlappingEntities(me);

                    // If the collisions are resolved, jump out of the loop
                    if (overlappingEntities.Count(y => !y.Collider.Trigger) == 0)
                    {
                        break;
                    }
                }

                me.Transform.Position = me.Transform.TargetPosition;
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

        public static Vector2 ResolveCollisions(Entity mover, List<Entity> collided)
        {
            foreach (Entity e in collided)
            {
                var resolvedBox = ResolveCollision(mover, e);
                mover.Collider.TargetBoundingBox = new Rectangle(resolvedBox.X, resolvedBox.Y, mover.Collider.TargetBoundingBox.Width, mover.Collider.TargetBoundingBox.Height);
            }

            return new Vector2(mover.Collider.TargetBoundingBox.X, mover.Collider.TargetBoundingBox.Y);
        }

        private static Point ResolveCollision(Entity e1, Entity e2)
        {
            // Determine directions traveling. 

            // Moving right = (0,0) position, but (1+,0) target
            // Moving down = (0,0) position but (0,1+)
            int xOffset = e1.Collider.WorldBoundingBox.X - e1.Collider.TargetBoundingBox.X;
            int yOffset = e1.Collider.WorldBoundingBox.Y - e1.Collider.TargetBoundingBox.Y;

            var totalOffset = new Point(
                xOffset == 0 ? 0 : -(xOffset / Math.Abs(xOffset)), // If moving left, X = -1
                yOffset == 0 ? 0 : -(yOffset / Math.Abs(yOffset))  // If moving up, X = -1
            );

            return new Point(
                EasyClamp(
                    ResolveDimension(e1.Collider.TargetBoundingBox.X, e1.Collider.TargetBoundingBox.Width, e2.Collider.TargetBoundingBox.X, e2.Collider.TargetBoundingBox.Width, totalOffset.X),
                    e1.Collider.WorldBoundingBox.X,
                    e1.Collider.TargetBoundingBox.X
                ),
                EasyClamp(
                    ResolveDimension(e1.Collider.TargetBoundingBox.Y, e1.Collider.TargetBoundingBox.Height, e2.Collider.TargetBoundingBox.Y, e2.Collider.TargetBoundingBox.Height, totalOffset.Y),
                    e1.Collider.WorldBoundingBox.Y,
                    e1.Collider.TargetBoundingBox.Y
                )
            );

        }

        private static int EasyClamp(int value, int i1, int i2)
        {
            if (i1 < i2)
                return Math.Clamp(value, i1, i2);
            else
                return Math.Clamp(value, i2, i1);
        }

        private static int ResolveDimension(int d1, int size1, int d2, int size2, int direction)
        {

            // Determine if X is overlapping
            // Is E1.X between E2.X and E2.X+Width
            // Or
            // Is E1.X+Width between E2.X and E2.X+Width
            if (d1 > d2 && d1 < d2 + size2)
            {
                // d1 Overlaps
            }
            else if (d1 + size1 > d2 && d1 + size1 < d2 + size2)
            {
                // d1+size1 overlaps
            }
            else
            {
                // Nothing overlaps, just kick it
                return d1;
            }

            // Determine which direction to punt the dimension
            if (direction < 0)
            {
                return d2 + size2;
            }
            else if (direction > 0)
            {
                return d2 - size1;
            }

            return d1;
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
