using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TopDownShooter.ECS.Managers;
using TopDownShooter.ECS.Components;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using TopDownShooter.Managers;
using Moq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using TopDownShooter.Interfaces;

namespace TopDownShooter.Tests.Managers
{
    [TestClass]
    public class PlayerManagerTests
    {
        #region | SetWeapon |
        [TestMethod]
        public void PlayerManager_SetWeapon_AddsComponent()
        {
            // Arrange
            var unit = new PlayerManager();

            // Act
            unit.SetWeapon(new Weapon());

            // Assert
            Assert.IsNotNull(unit.PlayerEntity.GetComponent<Weapon>());
        }

        [TestMethod]
        public void PlayerManager_SetWeapon_ReplacesComponent()
        {
            // Arrange
            var unit = new PlayerManager();
            var first = new Weapon() { Type = WeaponType.Shotgun };
            var second = new Weapon() { Type = WeaponType.Automatic };
            var startingCount = unit.PlayerEntity.ComponentCount;

            // Act
            unit.SetWeapon(first);
            unit.SetWeapon(second);

            // Assert
            Assert.IsNotNull(unit.PlayerEntity.GetComponent<Weapon>());
            Assert.AreEqual(unit.PlayerEntity.GetComponent<Weapon>().ID, second.ID, "The player entity should have the 2nd weapon as it's component, not the first");
            Assert.AreEqual(startingCount + 1, unit.PlayerEntity.ComponentCount, "Only one component should be added to the entity");
        }
        #endregion

        #region | SetSprite |
        [TestMethod]
        public void PlayerManager_SetSprite_Pass()
        {
            // Arrange
            var unit = new PlayerManager();
            var texture = new TextureRegion2D(null, 0, 0, 10, 10);

            // Act
            unit.SetSprite(texture);

            // Assert
            Assert.IsNotNull(unit.PlayerEntity.GetComponent<Sprite>());
        }
        #endregion

        #region | Update |
        [TestMethod]
        public void PlayerManager_Update_SetsVelocityDirection()
        {
            // Arrange
            var gameTime = new GameTime() { ElapsedGameTime = new TimeSpan(0, 0, 1) };
            var unit = new PlayerManager();
            var camera = new OrthographicCamera(new ViewportAdapterWrapper(null));
            unit.InputManager = new InputManagerWrapper();
            unit.Camera = camera;

            // Act
            unit.Update(gameTime);

            // Assert
            Assert.AreEqual(new Vector2(-1, -1), unit.PlayerEntity.GetComponent<Velocity>().Direction);
        }

        [TestMethod]
        public void PlayerManager_Update_VelocitiesCancelEachOther()
        {
            // Arrange
            var gameTime = new GameTime() { ElapsedGameTime = new TimeSpan(0, 0, 1) };
            var unit = new PlayerManager();
            var camera = new OrthographicCamera(new ViewportAdapterWrapper(null));
            unit.InputManager = new InputManagerWrapper() { MockIsKeyDown = (x) => true };
            unit.Camera = camera;

            // Act
            unit.Update(gameTime);

            // Assert
            Assert.AreEqual(Vector2.Zero, unit.PlayerEntity.GetComponent<Velocity>().Direction);
        }
        #endregion

    }
}
