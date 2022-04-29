using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Gui;
using TopDownShooter.Managers;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.BitmapFonts;
using TopDownShooter.Services;
using System.Diagnostics.CodeAnalysis;

namespace TopDownShooter
{
    public class Startup : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private StageManager _stageManager;
        private ContentCacheManager _contentManager;
        private GuiSystem _gui;
        private InputManager _inputManager;
        private readonly Guid _parentGuid;

        public Startup()
        {
            _graphics = new GraphicsDeviceManager(this);
#if DEBUG
            // this.Window.Position = new Point(-100, 100);
#endif

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _parentGuid = Guid.NewGuid();
        }

        protected override void Initialize()
        {
            base.Initialize();

            UpdateResolution(new Point(1920, 1080));
        }

        protected override void LoadContent()
        {
            MessagingService.Init();
            _inputManager = new InputManager();
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _contentManager = new ContentCacheManager(Content);
            WeaponService.Init(_contentManager);

            SetupDefaultUi();

            _stageManager = new StageManager(_spriteBatch, _contentManager, _gui, _inputManager);
            _stageManager.SetNextStage<Stages.MainMenu>();
        }

        protected override void Update(GameTime gameTime)
        {
#if DEBUG
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                Exit();
            }
            Debugger.ShowDebugInfo = _inputManager.IsKeyDown(KeyBinding.Debug);
#endif
            if ((_inputManager.DirectIsKeyDown(Keys.LeftAlt) || _inputManager.DirectIsKeyDown(Keys.RightAlt)) && _inputManager.DirectIsKeyDown(Keys.Enter))
            {
                _graphics.ToggleFullScreen();
            }


            // Order of operations is important. StageManager is dependent on InputManager getting up to date inputs, otherwise it'll be 1 frame behind
            _inputManager.Update(gameTime);
            _stageManager.Update(gameTime);
            _gui.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _stageManager.CurrentStage.Camera?.GetViewMatrix());

            _stageManager.CurrentStage.Draw();
            base.Draw(gameTime);
            _gui.Draw(gameTime);

            _spriteBatch.End();
        }

        private void SetupDefaultUi()
        {
            //TODO: Probably can pull out into a UserInterfaceManager
            var viewportAdapter = new DefaultViewportAdapter(GraphicsDevice);
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, () => Matrix.Identity);
            var font = Content.Load<BitmapFont>("Fonts\\KenPixel\\32\\KenPixel32");
            BitmapFont.UseKernings = false;
            Skin.CreateDefault(font);

            _gui = new GuiSystem(viewportAdapter, guiRenderer) { ActiveScreen = _contentManager.GetScreen(ScreenName.MainMenu) };

            MessagingService.Subscribe(EventType.UserInterface, Constants.UserInterface.SetActive, (sender, args) => { _gui.ActiveScreen = _contentManager.GetScreen((ScreenName)args); }, _parentGuid);
        }

        private void UpdateResolution(Point size)
        {
            _graphics.PreferredBackBufferWidth = size.X;
            _graphics.PreferredBackBufferHeight = size.Y;
            _graphics.IsFullScreen = false;

            _graphics.ApplyChanges();
            _gui.ClientSizeChanged();
        }
    }
}
