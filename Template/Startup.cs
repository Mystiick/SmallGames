using Microsoft.Xna.Framework;

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


}
