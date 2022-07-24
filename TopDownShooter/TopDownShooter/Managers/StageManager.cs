using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Gui;

using MystiickCore.Managers;
using MystiickCore.Interfaces;
using MystiickCore.Models;

namespace MystiickCore.Managers
{
    public class StageManager
    {

        private readonly SpriteBatch _spriteBatch;
        private readonly ContentCacheManager _contentCache;
        private readonly GuiSystem _gui;
        private readonly IInputManager _input;
        private readonly EntityComponentManager _ecm;

        public StageManager(SpriteBatch spriteBatch, ContentCacheManager contentManager, GuiSystem gui, IInputManager inputManager, EntityComponentManager ecm)
        {
            _spriteBatch = spriteBatch;
            _contentCache = contentManager;
            _gui = gui;
            _input = inputManager;
            _ecm = ecm;
        }

        public BaseStage CurrentStage
        {
            get;
            private set;
        }

        public BaseStage NextStage
        {
            get;
            private set;
        }

        public void SetNextStage<T>(object[]? arguments = null) where T : BaseStage, new()
        {
            NextStage = new T();
            NextStage.InitializeBase(_spriteBatch, this, _gui, _input, _ecm, arguments);
            NextStage.LoadContent(_contentCache);
        }

        public void Update(GameTime gameTime)
        {
            if (NextStage != null)
            {
                if (CurrentStage != null)
                {
                    // Dispose the current stage to free up any subscriptions it is holding onto before transitioning to the next scene
                    CurrentStage.Dispose();
                }

                CurrentStage = NextStage;
                CurrentStage.Start();
                NextStage = null;
            }

            _input.Update(gameTime);
            CurrentStage.Update(gameTime);
        }
    }
}
