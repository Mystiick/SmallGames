using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended.Gui;
using MystiickCore.Services;
using MystiickCore.Managers;

namespace MystiickCore.Models;

public class BaseGame : Game
{
    protected GraphicsDeviceManager _graphics;
    protected SpriteBatch _spriteBatch;
    protected StageManager _stageManager;
    protected ContentCacheManager _contentManager;
    protected GuiSystem? _gui;
    protected InputManager _inputManager;
    protected readonly Guid _parentGuid = Guid.NewGuid();
    protected EntityComponentManager _entityComponentManager = new();

    public BaseGame()
    {
        IsMouseVisible = true;
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
    }

    protected override void Initialize()
    {
        MessagingService.Init();

        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _contentManager = new ContentCacheManager(Content);
        _stageManager = new StageManager(_spriteBatch, _contentManager, _gui, _inputManager, _entityComponentManager);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        _stageManager.SetNextStage<EmptyStage>();
    }

    protected override void Update(GameTime gameTime)
    {
#if DEBUG
        if (Keyboard.GetState().IsKeyDown(Keys.Escape) && Keyboard.GetState().IsKeyDown(Keys.F1))
        {
            Exit();
        }
        Debugger.ShowDebugInfo = _inputManager.DirectIsKeyDown(Keys.OemTilde);
#endif
        if ((_inputManager.DirectIsKeyDown(Keys.LeftAlt) || _inputManager.DirectIsKeyDown(Keys.RightAlt)) && _inputManager.DirectIsKeyDown(Keys.Enter))
        {
            _graphics.ToggleFullScreen();
        }

        // Order of operations is important. StageManager is dependent on InputManager getting up to date inputs, otherwise it'll be 1 frame behind
        _inputManager.Update(gameTime);
        _stageManager.Update(gameTime);
        _gui?.Update(gameTime);

        base.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _stageManager.CurrentStage.Camera?.GetViewMatrix());

        _stageManager.CurrentStage.Draw();
        base.Draw(gameTime);
        _gui?.Draw(gameTime);

        _spriteBatch.End();
    }

    protected void UpdateResolution(Point size)
    {
        _graphics.PreferredBackBufferWidth = size.X;
        _graphics.PreferredBackBufferHeight = size.Y;
        _graphics.IsFullScreen = false;

        _graphics.ApplyChanges();

        _gui?.ClientSizeChanged();
    }
}
