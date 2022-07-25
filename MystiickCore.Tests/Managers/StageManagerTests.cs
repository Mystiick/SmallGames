using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Gui;

using MystiickCore.Interfaces;
using MystiickCore.Managers;
using MystiickCore.Models;

namespace MystiickCore.Tests.Managers
{
    /// <summary>
    /// Tests for <see cref="StageManager"/>
    /// </summary>
    [TestClass]
    public class StageManagerTests
    {
        [TestMethod]
        public void StageManager_SetNextScene_InitializesAndLoadsContent()
        {
            // Arrange
            var unit = new StageManager(null, null, null, null, new TestEntityComponentManager());

            // Act
            unit.SetNextStage<TestStage>();

            // Assert
            Assert.IsNotNull(unit.NextStage, "NextStage should be set to a new Stage");
            Assert.IsTrue(((TestStage)unit.NextStage).Initialized, "The stage should have been Initialized");
            Assert.IsTrue(((TestStage)unit.NextStage).ContentLoaded, "The Stage should have loaded content");
        }

        [TestMethod]
        public void StageManager_Update_UpdatesCurrentScene()
        {
            // Arrange
            var unit = new StageManager(null, null, null, new TestInputManager(), new TestEntityComponentManager()); // TODO: Mock

            // Act
            unit.SetNextStage<TestStage>();
            unit.Update(new GameTime());

            // Assert
            Assert.IsTrue(((TestStage)unit.CurrentStage).Updated, "CurrentStage should have been updated");
        }

        [TestMethod]
        public void StageManager_Update_SetsNextScene()
        {
            // Arrange
            var unit = new StageManager(null, null, null, new TestInputManager(), new TestEntityComponentManager()); // TODO: Mock

            // Act
            unit.SetNextStage<TestStage>();
            unit.Update(new GameTime());

            // Assert
            Assert.IsNotNull(unit.CurrentStage, "CurrentStage should be set to the new Stage");
            Assert.IsNull(unit.NextStage, "There should be no NextStage");

        }

        private class TestStage : BaseStage
        {
            public bool Initialized { get; private set; }
            public bool ContentLoaded { get; private set; }
            public bool Updated { get; private set; }

            public override void InitializeBase(SpriteBatch spriteBatch, StageManager stageManager, GuiSystem gui, IInputManager input, EntityComponentManager ecm, object[] args)
            {
                this.Initialized = true;
            }

            public override void LoadContent(ContentCacheManager contentManager)
            {
                this.ContentLoaded = true;
            }

            public override void Update(GameTime gameTime)
            {
                this.Updated = true;
            }
        }
    }
}
