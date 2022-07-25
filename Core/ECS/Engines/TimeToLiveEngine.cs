using Microsoft.Xna.Framework;
using MystiickCore.ECS.Components;

namespace MystiickCore.ECS.Engines
{
    public class TimeToLiveEngine : Engine
    {
        public override Type[] RequiredComponents => new[] { typeof(TimeToLive) };

        public override void Update(GameTime gameTime, List<Entity> allEntities)
        {
            for (int i = 0; i < this.Entities.Count; i++)
            {
                Entity entity = this.Entities[i];
                var ttl = entity.GetComponent<TimeToLive>();

                ttl.Lifespan -= gameTime.ElapsedGameTime.TotalSeconds;

                if (ttl.Lifespan <= 0)
                {
                    // Mark it for removal from all engines it's a part of
                    entity.Expired = true;
                }
            }

            // DO THIS LAST
            // Typically this would be done first, but for the TTL Engine, the Marked For Removals need to be determined first
            // If this is called first, any entities that are expired will hang around until the next frame and wouldn't be the end of the world
            base.Update(gameTime, allEntities);
        }
    }
}
