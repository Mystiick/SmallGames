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
        /// <summary>
        /// Reference to the currently active PlayerEntity. Updated every frame, so we don't need to worry about holding onto an expired entity
        /// </summary>
        public Entity PlayerEntity { get; set; }
        public TileGrid Grid { get; set; }

        protected Velocity EntityVelocity;

        protected bool EntityCanSeePlayer;

        public virtual void Update(GameTime gameTime, List<Entity> allEntities)
        {
            float distanceToPlayer = Vector2.Distance(CurrentEntity.Transform.Position, PlayerEntity.Transform.Position);
            EntityCanSeePlayer = CanSeePlayer(allEntities);
            EntityVelocity = CurrentEntity.GetComponent<Velocity>();

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

        public virtual void PlayerInformationChanged()
        {

        }

        private bool CanSeePlayer(List<Entity> allEntities)
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
