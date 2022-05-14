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
    public class TileEngineTests
    {

        #region " CanCalculateWeights "
        [TestMethod]
        public void TileEngine_CanCalculateWeights_InBounds()
        {
            // Arrange
            var unit = new TileEngine();
            var grid = new TileGrid() { Tiles = new Tile[10, 10] };
            Point[] input = new[] {
                 new Point(5, 5),
                 new Point(0, 0),
                 new Point(9, 9)
            };
            bool[] output;

            // Act
            output = new[] {
                unit.CanCalculateWeights(grid, input[0]),
                unit.CanCalculateWeights(grid, input[1]),
                unit.CanCalculateWeights(grid, input[2])
            };

            // Assert
            Assert.IsTrue(output[0]);
            Assert.IsTrue(output[1]);
            Assert.IsTrue(output[2]);
        }

        [TestMethod]
        public void TileEngine_CanCalculateWeights_X_OutOfBounds()
        {
            // Arrange
            var unit = new TileEngine();
            var grid = new TileGrid() { Tiles = new Tile[10, 10] };
            Point[] input = new[] {
                 new Point(10, 5),
                 new Point(100, 5),
                 new Point(-1, 5)
            };

            bool[] output;

            // Act
            output = new[] {
                unit.CanCalculateWeights(grid, input[0]),
                unit.CanCalculateWeights(grid, input[1]),
                unit.CanCalculateWeights(grid, input[2])
            };

            // Assert
            Assert.IsFalse(output[0]);
            Assert.IsFalse(output[1]);
            Assert.IsFalse(output[2]);
        }

        [TestMethod]
        public void TileEngine_CanCalculateWeights_Y_OutOfBounds()
        {
            // Arrange
            var unit = new TileEngine();
            var grid = new TileGrid() { Tiles = new Tile[10, 10] };
            Point[] input = new[] {
                 new Point(5, 10),
                 new Point(5, 100),
                 new Point(5, -1)
            };

            bool[] output;

            // Act
            output = new[] {
                unit.CanCalculateWeights(grid, input[0]),
                unit.CanCalculateWeights(grid, input[1]),
                unit.CanCalculateWeights(grid, input[2])
            };

            // Assert
            Assert.IsFalse(output[0]);
            Assert.IsFalse(output[1]);
            Assert.IsFalse(output[2]);
        }

        [TestMethod]
        public void TileEngine_CanCalculateWeights_XandY_OutOfBounds()
        {
            // Arrange
            var unit = new TileEngine();
            var grid = new TileGrid() { Tiles = new Tile[10, 10] };
            Point[] input = new[] {
                 new Point(10, 10),
                 new Point(100, 100),
                 new Point(-1, -1)
            };

            bool[] output;

            // Act
            output = new[] {
                unit.CanCalculateWeights(grid, input[0]),
                unit.CanCalculateWeights(grid, input[1]),
                unit.CanCalculateWeights(grid, input[2])
            };

            // Assert
            Assert.IsFalse(output[0]);
            Assert.IsFalse(output[1]);
            Assert.IsFalse(output[2]);
        }
        #endregion

        #region " Update "
        [TestMethod]
        public void TileEngine_Update_Pass()
        {
            // Arrange

            // Act

            // Assert
        }

        #endregion
    }
}
