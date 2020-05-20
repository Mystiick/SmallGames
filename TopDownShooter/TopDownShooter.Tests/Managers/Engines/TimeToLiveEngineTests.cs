using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using TopDownShooter.ECS;
using TopDownShooter.ECS.Components;
using TopDownShooter.ECS.Engines;
using TopDownShooter.Managers;

namespace TopDownShooter.Tests.Managers.Engines
{
    [TestClass]
    public class TimeToLiveEngineTests
    {
        [TestMethod]
        public void TimeToLiveEngine_Update_RemovesFromItself()
        {
            // Arrange
            var gameTime = new GameTime() { ElapsedGameTime = new TimeSpan(0, 0, 2) };
            var unit = new TimeToLiveEngine();
            unit.AddEntity(new Entity(new TimeToLive() { Lifespan = 1 }));

            // Act
            unit.Update(gameTime, null);

            // Assert
            Assert.AreEqual(0, unit.MyEntities.Count, "The Engine should have no more entities after they have been expired and updated again");
        }

    }
}
