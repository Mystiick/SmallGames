using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

using MystiickCore.Models;

namespace Template.UI;

public class MainMenuScreen : BaseScreen
{
    public MainMenuScreen() : base("UI\\MainMenu.xml") { }

    protected override void SetupEvents()
    {
        var quitButton = Screen.FindControl<Button>("btnQuit");
        quitButton.Clicked += (sender, args) => System.Environment.Exit(0);
    }
}
