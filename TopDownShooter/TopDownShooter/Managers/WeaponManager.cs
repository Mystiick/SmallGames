using System;
using Microsoft.Xna.Framework;

using MystiickCore.ECS;
using MystiickCore.ECS.Components;
using MystiickCore.Managers;

using TopDownShooter.ECS.Components;

namespace TopDownShooter.Managers;

public class WeaponManager
{
    readonly ContentCacheManager _contentCacheManager;
    readonly Random _rng;

    public WeaponManager(ContentCacheManager ccm, Random random = null)
    {
        _contentCacheManager = ccm;

        if (random == null)
        {
            random = new Random();
        }

        _rng = random;
    }

    public Entity[] GetBullets(Weapon weapon, Vector2 bulletDirection)
    {
        var output = new Entity[weapon.BulletsPerShot];

        for (int i = 0; i < weapon.BulletsPerShot; i++)
        {
            output[i] = GetNextBullet(weapon, bulletDirection);
        }

        return output;
    }

    private Entity GetNextBullet(Weapon weapon, Vector2 bulletDirection)
    {
        var angle = _rng.Next(-weapon.BulletSpread, weapon.BulletSpread) / 2;
        bulletDirection = Vector2.Transform(bulletDirection, Matrix.CreateRotationZ(MathHelper.ToRadians(angle)));

        Entity e = new Entity() { Name = Constants.Entities.Bullet };
        e.AddComponents(new Component[] {
            new Transform() { Position = weapon.Bullet.Owner.Transform.Position + weapon.Bullet.Owner.Sprite.Origin, Rotation = weapon.Bullet.Owner.Transform.Rotation },
            new Sprite() { Texture = _contentCacheManager.GetClippedAsset(AssetName.Bullet) },
            new Velocity() { Direction =  bulletDirection, Speed = Constants.Entities.BulletBaseSpeed * weapon.BulletSpeed },
            new BoxCollider() { LocalBoundingBox = Constants.Entities.BulletCollider, Trigger = true, Continuous = true, OnCollisionEnter = weapon.Bullet.OnBulletHit },
            new TimeToLive() { Lifespan = weapon.Range / weapon.BulletSpeed },
            weapon.Bullet
        });

        return e;
    }
}
