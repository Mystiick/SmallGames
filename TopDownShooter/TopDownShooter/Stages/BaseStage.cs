using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using TopDownShooter.Interfaces;
using TopDownShooter.Managers;

namespace TopDownShooter.Stages
{
    public abstract class BaseStage : IDisposable
    {
        protected GraphicsDevice GraphicsDevice;
        protected SpriteBatch SpriteBatch;
        protected MessagingManager MessagingManager;
        protected ContentCacheManager ContentCache;
        protected StageManager StageManager;
        protected GuiSystem UserInterface;
        protected EntityComponentManager EntityComponentManager;
        protected IInputManager InputManager;

        public OrthographicCamera Camera { get; protected set; }
        public readonly Guid StageID;

        protected BaseStage()
        {
            StageID = Guid.NewGuid();
        }

        public virtual void InitializeBase(
            SpriteBatch spriteBatch, 
            MessagingManager messagingManager, 
            StageManager stageManager, 
            GuiSystem gui,
            IInputManager input,
            object[] args)
        {
            GraphicsDevice = spriteBatch.GraphicsDevice;
            SpriteBatch = spriteBatch;
            MessagingManager = messagingManager;
            StageManager = stageManager;
            UserInterface = gui;
            InputManager = input;
            EntityComponentManager = new EntityComponentManager();
            EntityComponentManager.Init();
            Camera = new OrthographicCamera(GraphicsDevice) { Zoom = 3f };
        }

        /// <summary>
        /// Function that declares that this stage has the focus
        /// </summary>
        public virtual void Start()
        {

        }

        public virtual void LoadContent(ContentCacheManager contentManager)
        {
            ContentCache = contentManager;
        }

        public virtual void Update(GameTime gameTime)
        {
            EntityComponentManager.Update(gameTime);
        }

        public virtual void Draw()
        {
            EntityComponentManager.Draw(SpriteBatch, GraphicsDevice, Camera);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    MessagingManager.UnsubscribeParent(this.StageID);
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

    }
}
