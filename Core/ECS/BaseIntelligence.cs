using Microsoft.Xna.Framework;

using MystiickCore.ECS.Components;
using MystiickCore.ECS.Engines;

namespace MystiickCore.ECS;

public abstract class BaseIntelligence
{
    public Entity CurrentEntity { get; set; }
    /// <summary>
    /// Reference to the currently active PlayerEntity. Updated every frame, so we don't need to worry about holding onto an expired entity
    /// </summary>
    public Entity PlayerEntity { get; set; }

    protected Velocity EntityVelocity;

    protected bool EntityCanSeePlayer;

    protected List<Entity> AllEntities;

    public virtual void Update(GameTime gameTime, List<Entity> allEntities)
    {
        this.AllEntities = allEntities;

        EntityCanSeePlayer = CanSeePlayer(allEntities);
        EntityVelocity = CurrentEntity.GetComponent<Velocity>();
    }

    public virtual void PlayerInformationChanged()
    {

    }

    protected bool CanSeePlayer(List<Entity> allEntities)
    {
        // Shoot a ray toward the player
        Entity[] collidedEntities = PhysicsEngine.CastAllToward(CurrentEntity.Transform.Position, PlayerEntity.Transform.Position, allEntities);

        // If there are any Wall colliders hit, the NPC cannot see the player
        foreach (Entity e in collidedEntities)
        {
            if (e.Opaque)
            {
                return false;
            }
        }

        // Nothing is in the way, the NPC can see the player
        return true;
    }
}
