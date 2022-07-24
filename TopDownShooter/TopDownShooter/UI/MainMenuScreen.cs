using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

using MystiickCore;
using MystiickCore.Models;
using MystiickCore.Services;


namespace TopDownShooter.UI
{
    public class MainMenuScreen : BaseScreen
    {
        public MainMenuScreen() : base("UI\\MainMenu.xml") { }

        protected override void SetupEvents()
        {
            var quitButton = Screen.FindControl<Button>("btnQuit");
            quitButton.Clicked += (sender, args) => System.Environment.Exit(0);

            var playButton = Screen.FindControl<Button>("btnPlay");
            playButton.Clicked += (sender, args) => MessagingService.SendMessage(EventType.UserInterface, Constants.MainMenu.PlayButtonAction, this, null);

            var pathfinderButton = Screen.FindControl<Button>("btnPathfinder");
            pathfinderButton.Clicked += (sender, args) => MessagingService.SendMessage(EventType.UserInterface, Constants.MainMenu.PathfinderButtonAction, this, null);

#if DEBUG
            var spDebug = Screen.FindControl<StackPanel>("spDebug");
            spDebug.IsVisible = true;
#endif
        }
    }
}
