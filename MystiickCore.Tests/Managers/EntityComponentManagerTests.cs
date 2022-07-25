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

namespace MystiickCore.Tests.Managers;

[TestClass]
public class EntityComponentManagerTests
{
    #region | AddEngine |

    [TestMethod]
    public void EntityComponentManager_AddEngine_Pass()
    {
        // Arrange
        EntityComponentManager unit = new TestEntityComponentManager();

        // Act
        unit.AddEngine(new PhysicsEngine(), 0);

        // Assert
        Assert.AreEqual(1, unit.EngineCount, "Only one Engine should have been added to the manager");
    }

    [TestMethod]
    public void EntityComponentManager_AddEngine_DoesNotAddDuplicates()
    {
        // Arrange
        EntityComponentManager unit = new TestEntityComponentManager();
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
