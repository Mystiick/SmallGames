using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended.Gui;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.BitmapFonts;

using MystiickCore;
using MystiickCore.Services;
using MystiickCore.Managers;
using MystiickCore.Models;
using MystiickCore.ECS.Engines;

using TopDownShooter.ECS.Engines;
using TopDownShooter.Managers;
using TopDownShooter.UI;
using TopDownShooter.Services;

namespace TopDownShooter;

public class Startup : BaseGame
{

    public Startup()
    {
        
    }

    protected override void Initialize()
    {
        _inputManager = new InputManager(LoadKeybindings());

        base.Initialize();

        BuildEngines();
        SetupDefaultUi();
        UpdateResolution(new Point(1920, 1080));
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        WeaponService.Init(_contentManager);
        _stageManager.SetNextStage<Stages.MainMenu>();
    }

    private void SetupDefaultUi()
    {
        //TODO: Probably can pull out into a UserInterfaceManager
        var viewportAdapter = new DefaultViewportAdapter(GraphicsDevice);
        var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, () => Matrix.Identity);
        var font = Content.Load<BitmapFont>("Fonts\\KenPixel\\32\\KenPixel32");
        BitmapFont.UseKernings = false;
        Skin.CreateDefault(font);

        _gui = new GuiSystem(viewportAdapter, guiRenderer) { ActiveScreen = GetScreen(ScreenName.MainMenu) };

        MessagingService.Subscribe(EventType.UserInterface, Constants.UserInterface.SetActive, (sender, args) => { _gui.ActiveScreen = GetScreen((string)args); }, _parentGuid);
    }

    private void BuildEngines()
    {
        // Engines are processed in this order
        _entityComponentManager.AddEngine(new TimeToLiveEngine());
        _entityComponentManager.AddEngine(new HealthEngine());
        _entityComponentManager.AddEngine(new TileEngine()); 
        _entityComponentManager.AddEngine(new PhysicsEngine());
        _entityComponentManager.AddEngine(new IntelligenceEngine());
    }

    private Dictionary<string, MouseAndKeys> LoadKeybindings()
    {
        return new Dictionary<string, MouseAndKeys>
            {
                { KeyBinding.MoveUp, MouseAndKeys.W },
                { KeyBinding.MoveDown, MouseAndKeys.S },
                { KeyBinding.MoveLeft, MouseAndKeys.A },
                { KeyBinding.MoveRight, MouseAndKeys.D },
                { KeyBinding.Shoot, MouseAndKeys.LeftClick },
                { KeyBinding.Debug, MouseAndKeys.Tilde },
                { KeyBinding.ZoomIn, MouseAndKeys.R },
                { KeyBinding.ZoomOut, MouseAndKeys.F},
                { KeyBinding.WeaponOne, MouseAndKeys.One },
                { KeyBinding.WeaponTwo, MouseAndKeys.Two },
                { KeyBinding.WeaponThree, MouseAndKeys.Three },
            };
    }

    //TODO: Move to it's own class
    #region | Move to it's own class |
    private readonly Dictionary<string, BaseScreen> _screenCache = new Dictionary<string, BaseScreen>();
    public Screen GetScreen(string screen)
    {
        if (!_screenCache.ContainsKey(screen))
        {
            _screenCache.Add(screen, GetScreenByName(screen));
        }

        return _screenCache[screen].Screen;
    }


    private BaseScreen GetScreenByName(string screen)
    {
        BaseScreen output;

        switch (screen)
        {
            case ScreenName.None: output = new BlankScreen(); break;
            case ScreenName.MainMenu: output = new MainMenuScreen(); break;
            case ScreenName.Score: output = new ScoreScreen(); break;
            default: output = new BlankScreen(); break;
        }

        return output;
    }
    #endregion
}
