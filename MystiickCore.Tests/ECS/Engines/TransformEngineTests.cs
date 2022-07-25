using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

using MystiickCore.ECS;
using MystiickCore.ECS.Components;
using MystiickCore.ECS.Engines;

namespace MystiickCore.Tests.Managers.Engines;

[TestClass]
public class TransformEngineTests
{
    [TestMethod]
    public void VelocityEngine_Update_DoesNotSetVectorZeroToNaN_WithoutVelocity()
    {
        // Arrange
        var unit = new TransformEngine();
        var entity = new Entity(
            new Transform() { Position = Vector2.Zero }
        );
        var gameTime = new GameTime() { ElapsedGameTime = new System.TimeSpan(0, 0, 1) };

        unit.AddEntity(entity);

        // Act
        unit.Update(gameTime, null);

        // Assert
        Assert.IsFalse(float.IsNaN(entity.Transform.TargetPosition.X));
        Assert.IsFalse(float.IsNaN(entity.Transform.TargetPosition.Y));
    }

    [TestMethod]
    public void VelocityEngine_Update_DoesNotSetVectorZeroToNaN_WithVelocity()
    {
        // Arrange
        var unit = new TransformEngine();
        var entity = new Entity(
            new Transform() { Position = Vector2.One },
            new Velocity() { Direction = Vector2.Zero }
        );
        var gameTime = new GameTime() { ElapsedGameTime = new System.TimeSpan(0, 0, 1) };

        unit.AddEntity(entity);

        // Act
        unit.Update(gameTime, null);

        // Assert
        Assert.IsFalse(float.IsNaN(entity.Transform.TargetPosition.X));
        Assert.IsFalse(float.IsNaN(entity.Transform.TargetPosition.Y));
        Assert.AreEqual(Vector2.One, entity.Transform.TargetPosition);
    }

    [TestMethod]
    public void VelocityEngine_Update_UpdatesTargetPosition()
    {
        // Arrange
        var unit = new TransformEngine();
        var entity = new Entity(
            new Transform() { Position = Vector2.Zero },
            new Velocity() { Direction = new Vector2(5, 0), Speed = 1000f }
        );
        var gameTime = new GameTime() { ElapsedGameTime = new System.TimeSpan(0, 0, 1) };

        unit.AddEntity(entity);

        // Act
        unit.Update(gameTime, null);

        // Assert
        Assert.IsFalse(float.IsNaN(entity.Transform.TargetPosition.X));
        Assert.IsFalse(float.IsNaN(entity.Transform.TargetPosition.Y));
        Assert.AreEqual(new Vector2(1000, 0), entity.Transform.TargetPosition);
    }
}
