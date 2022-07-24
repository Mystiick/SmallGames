using Microsoft.VisualStudio.TestTools.UnitTesting;
using MystiickCore.ECS.Components;

namespace TopDownShooter.Tests.ECS.Components
{
    [TestClass]
    public class HealthTests
    {

        [TestMethod]
        public void Health_MaxHealth_Set_Pass()
        {
            // Arrange
            var unit1 = new Health(10);
            var unit2 = new Health() { MaxHealth = 20 };
            var unit3 = new Health(30) { MaxHealth = 40 };

            // Act

            // Assert
            Assert.AreEqual(10, unit1.MaxHealth);
            Assert.AreEqual(10, unit1.CurrentHealth);

            Assert.AreEqual(20, unit2.MaxHealth);
            Assert.AreEqual(20, unit2.CurrentHealth);

            Assert.AreEqual(40, unit3.MaxHealth);
            Assert.AreEqual(40, unit3.CurrentHealth);
        }

        [TestMethod]
        public void Health_MaxHealth_Set_AddsMaxAndCurrent()
        {
            // Arrange
            var unit = new Health(10);

            // Act
            unit.MaxHealth = 20;

            // Assert
            Assert.AreEqual(20, unit.MaxHealth);
            Assert.AreEqual(20, unit.CurrentHealth);
        }

        [TestMethod]
        public void Health_MaxHealth_Set_SubtractsMaxAndCurrent()
        {
            // Arrange
            var unit = new Health(10);

            // Act
            unit.MaxHealth = 5;

            // Assert
            Assert.AreEqual(5, unit.MaxHealth);
            Assert.AreEqual(5, unit.CurrentHealth);
        }

        [TestMethod]
        public void Health_MaxHealth_Set_EqualValuesDoNothing()
        {
            // Arrange
            var unit = new Health()
            {
                MaxHealth = 10
            };

            // Act
            unit.MaxHealth = 10;

            // Assert
            Assert.AreEqual(10, unit.MaxHealth);
            Assert.AreEqual(10, unit.CurrentHealth);
        }

    }
}
