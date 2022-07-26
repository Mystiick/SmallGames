using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;

using MystiickCore.ECS.Components;

namespace MystiickCore.ECS.Engines;

/// <summary>
/// Engine that handles collisions and the Transform.Position update for an entity
/// </summary>
public class PhysicsEngine : Engine
{
    private const int MAX_RESOLUTION_ATTEMPTS = 10;
    public override Type[] RequiredComponents => new Type[] { typeof(Transform) };

    /// <summary>At this point, all of the entities must have their target positions and bounding boxes set. This engine will update the final positions based on the targets</summary>
    public override void Update(GameTime gameTime, List<Entity> allEntities)
    {
        base.Update(gameTime, allEntities);

        ProcessTransforms(gameTime);
        ProcessColliders(gameTime);
    }

    private void ProcessTransforms(GameTime gameTime)
    {
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

                x.Transform.TargetPosition = x.Transform.Position + (v.Direction * v.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            else
            {
                x.Transform.TargetPosition = x.Transform.Position + Vector2.Zero * 0 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (x.Collider != null)
            {
                // Need to do it this way as x.Collider.TargetBoundingBox is a struct, and must be assigned to directly, instead of modified
                var targetBoundingBox = x.Collider.LocalBoundingBox;
                targetBoundingBox.X += (int)x.Transform.TargetPosition.X;
                targetBoundingBox.Y += (int)x.Transform.TargetPosition.Y;
                x.Collider.TargetBoundingBox = targetBoundingBox;

                // Need to do it this way as x.Collider.WorldBoundingBox is a struct, and must be assigned to directly, instead of modified
                var worldBoundingBox = x.Collider.LocalBoundingBox;
                worldBoundingBox.X += (int)x.Transform.Position.X;
                worldBoundingBox.Y += (int)x.Transform.Position.Y;
                x.Collider.WorldBoundingBox = worldBoundingBox;
            }
        }
    }

    private void ProcessColliders(GameTime gameTime)
    {
        for (int i = 0; i < this.Entities.Count; i++)
        {
            var x = this.Entities[i];

            if (x.HasComponent<BoxCollider>())
            {
                // Look for any non zero Target positions, and make sure we can actually move to that position
                List<Entity> overlappingEntities = Physics.GetOverlappingEntities(x, this.Entities);

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
    }

    private void HandleNonTriggerMovement(Entity me, IEnumerable<Entity> overlappingEntities)
    {
        // If a collider is static, it isn't ever going to move, no need to handle movement, something just ran into me
        if (!me.Collider.Static)
        {
            if (overlappingEntities.Any())
            {
                // Only try MAX_RESOLUTION_ATTEMPTS so we don't get stuck in an infinte loop resolving collisions
                for (int i = 0; i < MAX_RESOLUTION_ATTEMPTS; i++)
                {
                    // Resolve the current collision, and make sure the entity isn't colliding with another entity now
                    me.Transform.TargetPosition = Physics.ResolveCollisions(me, overlappingEntities.Where(x => !x.Collider.Trigger).ToList());
                    overlappingEntities = Physics.GetOverlappingEntities(me, this.Entities);

                    // If the collisions are resolved, jump out of the loop
                    if (!overlappingEntities.Any(y => !y.Collider.Trigger))
                    {
                        break;
                    }
                }
            }

            me.Transform.Position = me.Transform.TargetPosition;
        }
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
