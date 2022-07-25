using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using MonoGame.Extended;

using MystiickCore;
using MystiickCore.ECS;
using MystiickCore.ECS.Components;
using MystiickCore.Services;

using TopDownShooter.ECS.Components;

namespace TopDownShooter.Intelligences
{
    public abstract class ShooterIntelligence : BaseIntelligence
    {
        public TileGrid Grid { get; set; }

        public override void Update(GameTime gameTime, List<Entity> allEntities)
        {
            base.Update(gameTime, allEntities);

            float distanceToPlayer = Vector2.Distance(CurrentEntity.Transform.Position, PlayerEntity.Transform.Position);

            var weapon = CurrentEntity.GetComponent<Weapon>();
            if (weapon != null)
            {
                weapon.CooldownRemaining -= gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (EntityVelocity != null)
            {
                // Reset here, because it will get set any time the entity needs to move
                EntityVelocity.Speed = 0;

                if (EntityCanSeePlayer)
                {
                    // Rotate entity to face player
                    CurrentEntity.Transform.Rotation = Helpers.DetermineAngle(CurrentEntity.Transform.Position, PlayerEntity.Transform.Position);
                }
            }

            // If the player is closer than 100 units, shoot at them
            if (EntityCanSeePlayer && distanceToPlayer < 100 && weapon.CooldownRemaining <= 0)
            {
                ShootAtPlayer(weapon);
            }
        }

        protected void ShootAtPlayer(Weapon weapon)
        {
            if (weapon != null)
            {
                weapon.CooldownRemaining = weapon.ShotCooldown;

                // Create bullet entity
                Entity[] bullets = WeaponService.GetBullets(weapon, (PlayerEntity.Transform.Position - CurrentEntity.Transform.Position).NormalizedCopy());

                for (int i = 0; i < bullets.Length; i++)
                {
                    MessagingService.SendMessage(EventType.Spawn, "SpawnBullet", CurrentEntity, bullets[i]);
                }
            }
#if DEBUG 
            else
            {
                Console.WriteLine($"Entity {CurrentEntity.ID} cannot shoot. They have no Weapon Component");
            }
#endif
        }
    }
}
