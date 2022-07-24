using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using MystiickCore.ECS;
using MystiickCore.Managers;

using TopDownShooter.ECS.Components;
using TopDownShooter.Managers;

namespace MystiickCore.Services
{
    public static class WeaponService
    {
        public static WeaponManager Instance { get; private set; }

        /// <summary>
        /// Required one-time setup for the static class
        /// </summary>
        public static void Init(ContentCacheManager ccm, Random random = null)
        {
            Instance = new WeaponManager(ccm, random);
        }

        public static Entity[] GetBullets(Weapon weapon, Vector2 bulletDirection)
        {
            return Instance.GetBullets(weapon, bulletDirection);
        }

    }
}
