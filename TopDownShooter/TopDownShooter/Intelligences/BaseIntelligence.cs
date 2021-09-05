using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using MonoGame.Extended;

using TopDownShooter.ECS;
using TopDownShooter.ECS.Components;
using TopDownShooter.ECS.Engines;
using TopDownShooter.Services;

namespace TopDownShooter.Intelligences
{
    public abstract class BaseIntelligence
    {
        public Entity CurrentEntity { get; set; }
        public Entity PlayerEntity { get; set; }
        public TileGrid Grid { get; set; }

        public virtual void Update(GameTime gameTime, List<Entity> allEntities)
        {
            float distanceToPlayer = Vector2.Distance(CurrentEntity.Transform.Position, PlayerEntity.Transform.Position); 
            var v = CurrentEntity.GetComponent<Velocity>();
            var weapon = CurrentEntity.GetComponent<Weapon>();
            if (weapon != null) 
            {
                weapon.CooldownRemaining -= gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (v != null)
            {
                // Reset here, because it will get set any time the entity needs to move
                v.Speed = 0;

                if (CanSeePlayer(allEntities))
                {
                    // Rotate entity to face player
                    CurrentEntity.Transform.Rotation = Helpers.DetermineAngle(CurrentEntity.Transform.Position, PlayerEntity.Transform.Position);

                }
            }
            
            // If the player is closer than 100 units, shoot at them
            if (distanceToPlayer < 100 && weapon.CooldownRemaining <= 0)
            {
                ShootAtPlayer(weapon);
            }
        }

        protected bool CanSeePlayer(List<Entity> allEntities)
        {
            // Get distance between NPC and player
            float distance = Vector2.Distance(CurrentEntity.Transform.Position, PlayerEntity.Transform.Position);
            Vector2 direction = PlayerEntity.Transform.Position - CurrentEntity.Transform.Position;
            direction.Normalize();

            Entity[] collidedEntities;

            // Shoot a ray toward the player
            collidedEntities = PhysicsEngine.CastAll(CurrentEntity.Transform.Position, direction, distance, allEntities);

            // If there are any Wall colliders hit, the NPC cannot see the player
            foreach (Entity e in collidedEntities)
            {
                if (e.Name == Constants.Entities.Wall)
                {
                    return false;
                }
            }
            
            // Nothing is in the way, the NPC can see the player
            return true;
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
