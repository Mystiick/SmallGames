using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using TopDownShooter.ECS;
using TopDownShooter.ECS.Components;
using TopDownShooter.ECS.Engines;
using TopDownShooter.Exceptions;
using TopDownShooter.Managers;

namespace TopDownShooter.Tests.Managers
{
    [TestClass]
    public class EntityComponentManagerTests
    {
        #region | AddEngine |

        [TestMethod]
        public void EntityComponentManager_AddEngine_Pass()
        {
            // Arrange
            EntityComponentManager unit = new EntityComponentManager();

            // Act
            unit.AddEngine(new PhysicsEngine(), 0);

            // Assert
            Assert.AreEqual(1, unit.EngineCount, "Only one Engine should have been added to the manager");
        }

        [TestMethod]
        public void EntityComponentManager_AddEngine_DoesNotAddDuplicates()
        {
            // Arrange
            EntityComponentManager unit = new EntityComponentManager();
            DuplicateEngineProcessingOrderException expectedException = null;

            // Act
            try
            {
                unit.AddEngine(new PhysicsEngine(), 64);
                unit.AddEngine(new PhysicsEngine(), 64);
            }
            catch (DuplicateEngineProcessingOrderException ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException, $"Exception of type {nameof(DuplicateEngineProcessingOrderException)} should be thrown");
        }

        #endregion
    }
}
