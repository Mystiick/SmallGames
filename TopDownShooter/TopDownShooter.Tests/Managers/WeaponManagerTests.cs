using Microsoft.VisualStudio.TestTools.UnitTesting;
using TopDownShooter.Managers;
using TopDownShooter.ECS.Components;
using Microsoft.Xna.Framework;
using TopDownShooter.ECS;

namespace TopDownShooter.Tests.Managers
{
    /// <summary>
    /// Tests for <see cref="WeaponManager"/>
    /// </summary>
    [TestClass]
    public class WeaponManagerTests
    {

        [TestMethod]
        public void WeaponManager_GetBullets_ReturnsExpectedCount()
        {
            // Arrange
            var unit = new WeaponManager(new ContentCacheManagerWrapper(), new RandomWrapper());
            Entity[] output;
            Entity owner = new Entity(new Transform());
            Bullet bullet = new Bullet() { Owner = owner };
            Weapon weapon = new Weapon() { BulletsPerShot = 3, Bullet = bullet };
            var gameTime = new GameTime() { ElapsedGameTime = new System.TimeSpan(0, 0, 1) };

            // Act
            output = unit.GetBullets(gameTime, weapon, Vector2.One);

            // Assert
            Assert.IsNotNull(output);
            Assert.AreEqual(3, output.Length);
        }

    }

}