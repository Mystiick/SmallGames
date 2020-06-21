using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TopDownShooter.ECS;
using TopDownShooter.ECS.Components;

namespace TopDownShooter.Managers
{
    public class WeaponManager
    {
        readonly ContentCacheManager _contentCacheManager;
        readonly Random _rng;
        const float BASE_BULLET_SPEED = 500f;

        public WeaponManager(ContentCacheManager ccm, Random random = null)
        {
            _contentCacheManager = ccm;

            if (random == null)
            {
                random = new Random();
            }

            _rng = random;

        }

        public Entity[] GetBullets(GameTime gameTime, Weapon weapon, Vector2 bulletDirection)
        {
            var output = new Entity[weapon.BulletsPerShot];

            for (int i = 0; i < weapon.BulletsPerShot; i++)
            {
                output[i] = GetNextBullet(gameTime, weapon, bulletDirection);
            }

            return output;
        }

        private Entity GetNextBullet(GameTime gameTime, Weapon weapon, Vector2 bulletDirection)
        {            
            var angle = _rng.Next(-weapon.BulletSpread, weapon.BulletSpread) / 2;
            bulletDirection = Vector2.Transform(bulletDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(angle)));

            Entity e = new Entity();
            e.AddComponents(new Component[] {
                new Transform() { Position = weapon.Bullet.Owner.Transform.Position + weapon.Bullet.Owner.Sprite.Origin, Rotation = weapon.Bullet.Owner.Transform.Rotation },
                new Sprite() { Texture = _contentCacheManager.GetClippedAsset(AssetName.Bullet) },
                new Velocity() { Direction =  bulletDirection, Speed = BASE_BULLET_SPEED * weapon.BulletSpeed },
                new BoxCollider() { BoundingBox = new Rectangle(1, 0, 1, 1), Trigger = true, Continuous = true, OnCollisionEnter = weapon.Bullet.OnBulletHit },
                new TimeToLive() { Lifespan = weapon.Range / weapon.BulletSpeed },
                weapon.Bullet
            });
            e.Name = "Bullet";

            return e;
        }
    }
}
