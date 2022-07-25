using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

using MystiickCore.ECS;
using MystiickCore.ECS.Components;
using MystiickCore.ECS.Engines;
using MystiickCore.Exceptions;
using MystiickCore.Managers;

namespace MystiickCore.Tests.EndToEnd.Managers;

/// <summary>
/// End to End Tests for <see cref="EntityComponentManager"/> and its engines
/// </summary>
[TestClass, TestCategory("End to End")]
public class E2E_EntityComponentManagerTests
{
    // These are used in every End to End test
    EntityComponentManager ecs;
    TimeToLiveEngine ttlEngine;
    TransformEngine velocityEngine;
    PhysicsEngine physicsEngine;
    GameTime gameTime;

    [TestInitialize]
    public void TestInitialize()
    {
        ecs = new TestEntityComponentManager();
        ttlEngine = new TimeToLiveEngine();
        velocityEngine = new TransformEngine();
        physicsEngine = new PhysicsEngine();
        gameTime = new GameTime() { ElapsedGameTime = new TimeSpan(0, 0, 1) };

        TestHelper.AddEngines(ecs, ttlEngine, velocityEngine, physicsEngine);

    }

    #region | TimeToLiveEngine |

    [TestMethod]
    public void E2E_EntityComponentManager_TimeToLiveEngine_ExpiresEntities()
    {
        // Arrange
        var entity1 = new Entity(new TimeToLive() { Lifespan = 1 }, new Transform());
        var entity2 = new Entity(new Transform());

        ecs.AddEntity(entity1);
        ecs.AddEntity(entity2);

        // Act
        ecs.Update(gameTime);

        // Assert
        Assert.AreEqual(1, velocityEngine.MyEntities.Count, "2 seconds have passed, there should be exactly one valid entity");
        Assert.AreEqual(1, ecs.MyEntities.Count, "The expired entity should not be in the ECS Manager");
        Assert.IsTrue(entity1.Expired, "2 seconds have passed, Entity1 should be expired");
        Assert.IsFalse(entity2.Expired, "Entity2 has no TTL and should not be expired");
    }

    [TestMethod]
    public void E2E_EntityComponentManager_Collision_TruePositionUpdates()
    {
        // Arrange
        var entity = new Entity(
            new Transform() { Position = Vector2.Zero },
            new Velocity() { Direction = new Vector2(5, 0), Speed = 1000f }, // NOTE: This is set to 5 intentionally. The direction should be Normalized, converting into (1, 0) in this scenario
            new BoxCollider()
        );
        ecs.AddEntity(entity);

        // Act
        ecs.Update(gameTime);

        // Assert
        Assert.AreEqual(new Vector2(1000, 0), entity.Transform.Position, "The X value should be 1000, as 1000 milliseconds have \"passed\" (as far as gameTime is concerned).");
        Assert.AreEqual(entity.Transform.Position, entity.Transform.TargetPosition, "The transform should have moved to its target position as there is no collision");
    }

    [TestMethod]
    public void E2E_EntityComponentManager_Collision_TruePositionDoesNotUpdateOnWouldBeCollision()
    {
        // Arrange
        var entity = new Entity(
            new Transform() { Position = Vector2.Zero },
            new Velocity() { Direction = new Vector2(5, 0), Speed = 1000f },
            new BoxCollider() { LocalBoundingBox = new Rectangle(0, 0, 10, 10) }
        );
        ecs.AddEntity(entity);

        var collidedEntity = new Entity(
            new Transform() { Position = new Vector2(500, -100) },
            new BoxCollider() { LocalBoundingBox = new Rectangle(0, 0, 1000, 1000) }
        );
        ecs.AddEntity(collidedEntity);

        // Act
        ecs.Update(gameTime);

        // Assert
        Assert.AreEqual(new Vector2(490, 0), entity.Transform.Position, "Entity should resolve collision. collidedEntity is at 500, and entity is size of 10. 500-10");
    }

    [TestMethod]
    public void E2E_EntityComponentManager_Collision_CantMoveX_ButStillMovesY()
    {
        // Arrange
        var entity = new Entity(
            new Transform() { Position = Vector2.Zero },
            new Velocity() { Direction = new Vector2(1, 1), Speed = 1000f },
            new BoxCollider() { LocalBoundingBox = new Rectangle(0, 0, 10, 10) }
        );
        ecs.AddEntity(entity);

        var collidedEntity = new Entity(
            new Transform() { Position = new Vector2(500, -100) },
            new BoxCollider() { LocalBoundingBox = new Rectangle(0, 0, 1000, 1000) }
        );
        ecs.AddEntity(collidedEntity);

        // Act
        ecs.Update(gameTime);

        // Assert
        Assert.AreEqual(entity.Transform.TargetPosition.Y, entity.Transform.Position.Y, "Entity should be able to move to its Y target");
        Assert.AreEqual(490, entity.Transform.Position.X, "Entity should resolve X target to 490");
    }

    [TestMethod]
    public void E2E_EntityComponentManager_Collision_ResolveY_MovesX()
    {
        // Arrange
        var entity = new Entity(
            new Transform() { Position = Vector2.Zero },
            new Velocity() { Direction = new Vector2(1, 1), Speed = 1000f },
            new BoxCollider() { LocalBoundingBox = new Rectangle(0, 0, 10, 10) }
        );
        ecs.AddEntity(entity);

        var collidedEntity = new Entity(
            new Transform() { Position = new Vector2(-100, 500) },
            new BoxCollider() { LocalBoundingBox = new Rectangle(0, 0, 1000, 1000) }
        );
        ecs.AddEntity(collidedEntity);

        // Act
        ecs.Update(gameTime);

        // Assert
        Assert.AreEqual(entity.Transform.TargetPosition.X, entity.Transform.Position.X, "Entity should be able to move to its X target");
        Assert.AreEqual(490, entity.Transform.Position.Y, "Entity should resolve its Y target to 490");
    }

    [TestMethod]
    public void E2E_EntityComponentManager_Collision_CollisionActionTriggers()
    {
        // Arrange
        bool success = false;

        var entity = new Entity(
            new Transform() { Position = Vector2.Zero },
            new Velocity() { Direction = new Vector2(1, 1), Speed = 1000f },
            new BoxCollider() { LocalBoundingBox = new Rectangle(0, 0, 10, 10), Trigger = true, OnCollisionEnter = (a, b) => { success = true; } }
        );
        ecs.AddEntity(entity);

        var collidedEntity = new Entity(
            new Transform() { Position = new Vector2(-100, 500) },
            new BoxCollider() { LocalBoundingBox = new Rectangle(0, 0, 1000, 1000) }
        );
        ecs.AddEntity(collidedEntity);

        // Act
        ecs.Update(gameTime);

        // Assert
        Assert.IsTrue(success);
    }

    [TestMethod]
    public void E2E_EntityComponentManager_Collision_Continuous_RayCollision_NoCollisionAndNoTrigger()
    {
        // Arrange
        bool success = true;

        var entity = new Entity(
            new Transform() { Position = Vector2.Zero },
            new Velocity() { Direction = new Vector2(1, 0), Speed = 1000f },
            new BoxCollider() { LocalBoundingBox = new Rectangle(0, 0, 10, 10), Continuous = true, Trigger = true, OnCollisionEnter = (a, b) => { success = false; } }
        )
        {
            Name = "Unit"
        };
        ecs.AddEntity(entity);

        var collidedEntity = new Entity(
            new Transform() { Position = new Vector2(-1, 500) },
            new BoxCollider() { LocalBoundingBox = new Rectangle(0, 0, 10, 10) }
        )
        {
            Name = "Other"
        };
        ecs.AddEntity(collidedEntity);

        // Act
        ecs.Update(gameTime);

        // Assert
        Assert.IsTrue(success);
    }

    [TestMethod]
    public void E2E_EntityComponentManager_Collision_Continuous_RayCollision_RayTrigger()
    {
        // Arrange
        bool success = true;
        bool unitCollided = false;
        bool otherHadCollision = false;

        var entity = new Entity(
            new Transform() { Position = Vector2.Zero },
            new Velocity() { Direction = new Vector2(1, 0), Speed = 1000f },
            new BoxCollider()
            {
                LocalBoundingBox = new Rectangle(0, 0, 10, 10),
                Continuous = true,
                Trigger = true,
                OnCollisionEnter = (a, b) => { unitCollided = true; },
                OnCollisionHitMe = (a, b) => { success = false; }
            }
        )
        {
            Name = "Unit"
        };
        ecs.AddEntity(entity);

        var collidedEntity = new Entity(
            new Transform() { Position = new Vector2(500, -1) },
            new BoxCollider()
            {
                LocalBoundingBox = new Rectangle(0, 0, 10, 10),
                OnCollisionEnter = (a, b) => { success = false; },
                OnCollisionHitMe = (a, b) => { otherHadCollision = true; }
            }
        )
        {
            Name = "Other"
        };
        ecs.AddEntity(collidedEntity);

        // Act
        ecs.Update(gameTime);

        // Assert
        Assert.IsTrue(success, "Only the expected actions should be invoked. Nothing should hit 'Unit', and 'Other' is not overlapping with anything");
        Assert.IsTrue(unitCollided, "'Unit' collided with 'Other', and the action should be invoked");
        Assert.IsTrue(otherHadCollision, "There was a collision with 'Other', and the action should be invoked");
    }
    #endregion

}
