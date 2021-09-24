using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using TopDownShooter.ECS.Components;
using TopDownShooter.Services;

namespace TopDownShooter.ECS.Engines
{
    public class HealthEngine : Engine
    {
        public override Type[] RequiredComponents => new[] { typeof(Health) };

        public override void Update(GameTime gameTime, List<Entity> allEntities)
        {
            base.Update(gameTime, allEntities);

            for (int i = 0; i < this.Entities.Count; i++)
            {
                var e = this.Entities[i];
                var h = e.GetComponent<Health>();

                if (h.CurrentHealth <= 0)
                {
                    e.Expired = true;

                    switch (e.Type)
                    {
                        case EntityType.Enemy:
                            MessagingService.SendMessage(EventType.GameEvent, Constants.GameEvent.EnemyKilled, e);
                            break;
                        case EntityType.Player:
                            MessagingService.SendMessage(EventType.GameEvent, Constants.GameEvent.PlayerKilled, e);
                            break;
                    }
                }
            }
        }
    }
}
