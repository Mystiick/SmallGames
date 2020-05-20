using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TopDownShooter.ECS.Components
{
    public class Weapon : Component
    {
        public WeaponType Type { get; set; }
        public double ShotCooldown { get; set; }
        public double Range { get; set; }
        public double CooldownRemaining { get; set; }
        public Bullet Bullet { get; set; }
        /// <summary>Angle in Degrees</summary>
        public int BulletSpread { get; set; }
        public int AmmoCapacity { get; set; }
        public int CurrentAmmo { get; set; }
        public int BulletsPerShot { get; set; }
        public int BulletSpeed { get; set; }
    }
}
