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
    public class PhysicsEngineTests
    {

        #region | GetOverlappingEntities |
        /// <summary>Tests that adjacent colliders are not intersecting, and that entities don't collide with themselves</summary>
        [TestMethod]
        public void PhysicsEngine_GetOverlappingEntities_NoCollisions()
        {
            // Arrange
            var unit = new PhysicsEngine();
            var entity1 = new Entity();
            var entity2 = new Entity();
            List<Entity> output;

            entity1.AddComponent(CreateCollider(0, 0));
            entity2.AddComponent(CreateCollider(10, 0));

            unit.AddEntity(entity1);
            unit.AddEntity(entity2);

            // Act
            output = unit.GetOverlappingEntities(entity1);

            // Assert
            Assert.AreEqual(0, output.Count, "No entities are colliding, so they shouldn't be overlapping.");
        }

        /// <summary>Tests that adjacent colliders are not intersecting, and that entities don't collide with themselves</summary>
        [TestMethod]
        public void PhysicsEngine_GetOverlappingEntities_OneCollision()
        {
            // Arrange
            var unit = new PhysicsEngine();
            var entity1 = new Entity();
            var entity2 = new Entity();
            List<Entity> output;

            entity1.AddComponent(CreateCollider(0, 0));
            entity2.AddComponent(CreateCollider(10, 0));

            entity1.Collider.TargetBoundingBox = new Rectangle(5, 0, 10, 10);

            unit.AddEntity(entity1);
            unit.AddEntity(entity2);

            // Act
            output = unit.GetOverlappingEntities(entity1);

            // Assert
            Assert.AreEqual(1, output.Count, "No entities are colliding, so they shouldn't be overlapping.");
        }
        #endregion

        #region | ResolveCollisions |
        /// <summary>
        /// Tests that the following scenario resolves properly, given the same Y and 1 &amp; 2 are the TargetBoundingBoxes for entity 1 &amp; 2
        /// X: ..........1111111111
        /// X: ...............2222222222
        /// </summary>
        [TestMethod]
        public void PhysicsEngine_ResolveCollisions_EntityOverlapsFromLeft()
        {
            // Arrange
            var unit = new PhysicsEngine();
            var entity1 = new Entity();
            var entity2 = new Entity();
            Vector2 output;

            entity1.AddComponent(CreateCollider(0, 0));
            entity2.AddComponent(CreateCollider(15, 0));

            entity1.Collider.TargetBoundingBox = new Rectangle(10, 0, 10, 10);

            unit.AddEntity(entity1);
            unit.AddEntity(entity2);

            // Act
            output = PhysicsEngine.ResolveCollisions(entity1, new List<Entity>() { entity2 });

            // Assert
            Assert.AreEqual(new Vector2(5, 0), output, "The target bounding box overlaps by 5 pixels and must be pushed 5 units left");
        }
        /// <summary>
        /// Tests that the following scenario resolves properly, given the same Y and 1 &amp; 2 are the TargetBoundingBoxes for entity 1 &amp; 2
        /// X: ..........1111111111
        /// X: .....2222222222
        /// </summary>
        [TestMethod]
        public void PhysicsEngine_ResolveCollisions_EntityOverlapsFromRight()
        {
            // Arrange
            var unit = new PhysicsEngine();
            var entity1 = new Entity();
            var entity2 = new Entity();
            Vector2 output;

            entity1.AddComponent(CreateCollider(20, 0));
            entity2.AddComponent(CreateCollider(5, 0));

            entity1.Collider.TargetBoundingBox = new Rectangle(10, 0, 10, 10);

            unit.AddEntity(entity1);
            unit.AddEntity(entity2);

            // Act
            output = PhysicsEngine.ResolveCollisions(entity1, new List<Entity>() { entity2 });

            // Assert
            Assert.AreEqual(new Vector2(15, 0), output, "The target bounding box overlaps by 5 pixels and must be pushed 5 units right");
        }

        /// <summary>
        /// Tests that the following scenario resolves properly, given the same Y and 1 &amp; 2 are the TargetBoundingBoxes for entity 1 &amp; 2
        /// Y: ..........1111111111
        /// Y: ...............2222222222
        /// </summary>
        [TestMethod]
        public void PhysicsEngine_ResolveCollisions_EntityOverlapsFromTop()
        {
            // Arrange
            var unit = new PhysicsEngine();
            var entity1 = new Entity();
            var entity2 = new Entity();
            Vector2 output;

            entity1.AddComponent(CreateCollider(0, 0));
            entity2.AddComponent(CreateCollider(0, 15));

            entity1.Collider.TargetBoundingBox = new Rectangle(0, 10, 10, 10);

            unit.AddEntity(entity1);
            unit.AddEntity(entity2);

            // Act
            output = PhysicsEngine.ResolveCollisions(entity1, new List<Entity>() { entity2 });

            // Assert
            Assert.AreEqual(new Vector2(0, 5), output, "The target bounding box overlaps by 5 pixels and must be pushed 5 units left");
        }
        /// <summary>
        /// Tests that the following scenario resolves properly, given the same Y and 1 &amp; 2 are the TargetBoundingBoxes for entity 1 &amp; 2
        /// Y: ..........1111111111
        /// Y: .....2222222222
        /// </summary>
        [TestMethod]
        public void PhysicsEngine_ResolveCollisions_EntityOverlapsFromBottom()
        {
            // Arrange
            var unit = new PhysicsEngine();
            var entity1 = new Entity();
            var entity2 = new Entity();
            Vector2 output;

            entity1.AddComponent(CreateCollider(0, 20));
            entity2.AddComponent(CreateCollider(0, 5));

            entity1.Collider.TargetBoundingBox = new Rectangle(0, 10, 10, 10);

            unit.AddEntity(entity1);
            unit.AddEntity(entity2);

            // Act
            output = PhysicsEngine.ResolveCollisions(entity1, new List<Entity>() { entity2 });

            // Assert
            Assert.AreEqual(new Vector2(0, 15), output, "The target bounding box overlaps by 5 pixels and must be pushed 5 units right");
        }
        #endregion

        #region | Helper Methods |
        private BoxCollider CreateCollider(int x, int y)
        {
            return new BoxCollider()
            {
                LocalBoundingBox = new Rectangle(x, y, 10, 10),
                TargetBoundingBox = new Rectangle(x, y, 10, 10),
                WorldBoundingBox = new Rectangle(x, y, 10, 10)
            };
        }
        #endregion
    }
}