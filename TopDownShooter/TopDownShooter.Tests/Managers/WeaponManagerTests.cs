using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

using MystiickCore.ECS;
using MystiickCore.ECS.Components;

using TopDownShooter.Managers;
using TopDownShooter.ECS.Components;

namespace TopDownShooter.Tests.Managers;

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
        Entity owner = new(new Transform(), new Sprite());
        Bullet bullet = new() { Owner = owner };
        Weapon weapon = new() { BulletsPerShot = 3, Bullet = bullet };

        // Act
        output = unit.GetBullets(weapon, Vector2.One);

        // Assert
        Assert.IsNotNull(output);
        Assert.AreEqual(3, output.Length);
    }

}
