using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

using MystiickCore.ECS;
using MystiickCore.ECS.Components;
using MystiickCore.ECS.Engines;
using MystiickCore.Managers;

namespace MystiickCore.Tests.Managers.Engines;

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
