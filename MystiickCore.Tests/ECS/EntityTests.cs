using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

using MystiickCore.ECS;
using MystiickCore.ECS.Components;
using MystiickCore.Exceptions;

namespace MystiickCore.Tests.ECS
{
    [TestClass]
    public class EntityTests
    {
        #region | AddComponent |
        [TestMethod]
        public void Entity_AddComponent_Pass()
        {
            // Arrange
            Entity unit = new Entity();

            // Act
            unit.AddComponent(new TestComponent());

            // Assert
            Assert.AreEqual(1, unit.ComponentCount, "The Envity should only have exactly 1 component");
        }

        [TestMethod]
        public void Entity_AddComponent_AddingTransformSetsProperty()
        {
            // Arrange
            Entity unit = new Entity();
            Vector2 expectedPosition = new Vector2(100, 200);

            // Act
            unit.AddComponent(new Transform() { Position = expectedPosition });

            // Assert
            Assert.AreEqual(1, unit.ComponentCount, "The Envity should only have exactly 1 component");
            Assert.AreEqual(expectedPosition, unit.Transform.Position, "The entity should have set the position equal to the expectedPosition");
        }

        [TestMethod]
        public void Entity_AddComponent_CannotAddDuplicateComponent()
        {
            // Arrange
            Entity unit = new Entity();
            DuplicateComponentException expectedException = null;

            // Act
            unit.AddComponent(new TestComponent());
            try
            {
                unit.AddComponent(new TestComponent());
            }
            catch (DuplicateComponentException ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException, "expectedException should be set");
        }

        [TestMethod]
        public void Entity_AddComponent_CannotAddDuplicateComponentsInConstructor()
        {
            // Arrange
            Entity unit;
            DuplicateComponentException expectedException = null;

            // Act
            try
            {
                unit = new Entity(new TestComponent(), new TestComponent());
            }
            catch (DuplicateComponentException ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException, "expectedException should be set");
        }

        [TestMethod]
        public void Entity_AddComponent_AddMultiplePass()
        {
            // Arrange
            Entity unit = new Entity();

            // Act
            unit.AddComponent(new TestComponent());
            unit.AddComponent(new Transform());

            // Assert
            Assert.AreEqual(2, unit.ComponentCount, "The Envity should only have exactly 2 components");
        }

        [TestMethod]
        public void Entity_AddComponent_AddMultipleInConstructorPass()
        {
            // Arrange
            Entity unit;

            // Act
            unit = new Entity(new Transform(), new TestComponent());

            // Assert
            Assert.AreEqual(2, unit.ComponentCount, "The Envity should only have exactly 2 components");
        }
        #endregion

        #region | GetComponent |
        [TestMethod]
        public void Entity_GetComponent_Pass()
        {
            // Arrange
            Entity unit = new Entity(new TestComponent(), new Transform());
            Component expectedComponent;

            // Act
            expectedComponent = unit.GetComponent<TestComponent>();

            // Assert
            Assert.IsNotNull(expectedComponent, "GetComponent should find expectedComponent");
            Assert.AreEqual(typeof(TestComponent), expectedComponent.GetType(), "The expectedComponent should get a TestComponent type");
        }

        [TestMethod]
        public void Entity_GetComponent_ReturnsNullWhenNoMatchFound()
        {
            // Arrange
            Entity unit = new Entity();
            Component expectedComponent;

            // Act
            expectedComponent = unit.GetComponent<TestComponent>();

            // Assert
            Assert.IsNull(expectedComponent, "Component should be null if none is found");
        }

        [TestMethod]
        public void Entity_GetComponent_ModifyingPropertyPersists()
        {
            // Arrange
            Entity unit = new Entity(new TestComponent() { Pass = false }, new Transform());
            TestComponent expectedComponent;

            // Act
            expectedComponent = unit.GetComponent<TestComponent>();
            expectedComponent.Pass = true;
            expectedComponent = unit.GetComponent<TestComponent>();

            // Assert
            Assert.IsNotNull(expectedComponent, "GetComponent should find expectedComponent");
            Assert.AreEqual(true, expectedComponent.Pass, "Values should persist when updated");
        }
        #endregion

        #region | HasComponent |
        [TestMethod]
        public void Entity_HasComponent_TruePass()
        {
            // Arrange
            Entity unit = new Entity(new TestComponent());
            bool value;

            // Act
            value = unit.HasComponent(typeof(TestComponent));

            // Assert
            Assert.AreEqual(true, value, "The Envity should have a TestComponent");
        }

        [TestMethod]
        public void Entity_HasComponent_FalsePass()
        {
            // Arrange
            Entity unit = new Entity();
            bool value;

            // Act
            value = unit.HasComponent(typeof(TestComponent));

            // Assert
            Assert.AreEqual(false, value, "The Envity should not have a TestComponent");
        }
        #endregion
    }

    public class TestComponent : Component
    {
        public bool Pass = false;
    }
}
