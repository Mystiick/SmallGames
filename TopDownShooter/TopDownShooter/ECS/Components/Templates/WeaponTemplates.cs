using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownShooter.ECS.Components.Templates
{
    public static class WeaponTemplates
    {
        public static Weapon Pistol(Entity owner)
        {
            return new Weapon()
            {
                Type = WeaponType.Single,
                Range = 1,
                ShotCooldown = .3f,
                BulletSpread = 2,
                AmmoCapacity = 12,
                CurrentAmmo = 12,
                BulletSpeed = 1,
                BulletsPerShot = 1,
                Bullet = new Bullet()
                {
                    Damage = 10,
                    Owner = owner,
                    OnBulletHit = DefaultBulletHit
                }
            };
        }

        public static Weapon AssaultRifle(Entity owner)
        {
            return new Weapon()
            {
                Type = WeaponType.Automatic,
                Range = 1,
                ShotCooldown = .1f,
                BulletSpread = 5,
                AmmoCapacity = 50,
                CurrentAmmo = 50,
                BulletSpeed = 1,
                BulletsPerShot = 1,
                Bullet = new Bullet()
                {
                    Damage = 3,
                    Owner = owner,
                    OnBulletHit = DefaultBulletHit
                }
            };
        }

        public static Weapon Shotgun(Entity owner)
        {
            return new Weapon()
            {
                Type = WeaponType.Shotgun,
                Range = .5,
                ShotCooldown = 1f,
                BulletSpread = 30,
                AmmoCapacity = 6,
                CurrentAmmo = 6,
                BulletSpeed = 2,
                BulletsPerShot = 5,
                Bullet = new Bullet()
                {
                    Damage = 10,
                    Owner = owner,
                    OnBulletHit = DefaultBulletHit
                }
            };
        }

        public static Action<Entity, Entity> DefaultBulletHit => (me, other) =>
        {
            var bullet = me.GetComponent<Bullet>();

            if (bullet?.Owner?.ID != other.ID && other.Name != "Bullet")
            {
                me.Expired = true;

                var health = other.GetComponent<Health>();

                if (health != null)
                {
                    health.CurrentHealth -= bullet.Damage;
                    Console.WriteLine($"{other.ID}: {health.CurrentHealth}");
                }
            }
        };
        
    }
}
