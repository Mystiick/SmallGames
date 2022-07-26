using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui;
using MonoGame.Extended.ViewportAdapters;

using MystiickCore;
using MystiickCore.Managers;
using MystiickCore.Models;

using Template.UI;

namespace Template;

internal class Startup : BaseGame
{
    protected override void Initialize()
    {
        _inputManager = new InputManager(LoadKeybindings());

        base.Initialize();
        SetupDefaultUi();
    }

    private Dictionary<string, MouseAndKeys> LoadKeybindings()
    {
        return new Dictionary<string, MouseAndKeys>
            {
                { KeyBinding.MoveUp, MouseAndKeys.W },
                { KeyBinding.MoveDown, MouseAndKeys.S },
                { KeyBinding.MoveLeft, MouseAndKeys.A },
                { KeyBinding.MoveRight, MouseAndKeys.D },
                { KeyBinding.Debug, MouseAndKeys.Tilde }
            };
    }

    private void SetupDefaultUi()
    {
        //TODO: Probably can pull out into a UserInterfaceManager
        var viewportAdapter = new DefaultViewportAdapter(GraphicsDevice);
        var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, () => Matrix.Identity);
        var font = Content.Load<BitmapFont>("Fonts\\KenPixel\\32\\KenPixel32");
        BitmapFont.UseKernings = false;
        Skin.CreateDefault(font);

        _gui = new GuiSystem(viewportAdapter, guiRenderer) { ActiveScreen = new MainMenuScreen().Screen };
    }

}
