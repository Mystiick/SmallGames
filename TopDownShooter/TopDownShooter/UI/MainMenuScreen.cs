using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Markup;
using TopDownShooter.Managers;

namespace TopDownShooter.UI
{
    public class MainMenuScreen : IScreen
    {
        private readonly string markupPath = "UI\\MainMenu.xml";
        private readonly MessagingManager _messageManager;

        public Screen Screen { get; private set; }

        public MainMenuScreen(MessagingManager messagingManager)
        {
            _messageManager = messagingManager;
            LoadFromMarkup();
        }

        public void LoadFromMarkup()
        {
            var parser = new MarkupParser();
            this.Screen = new Screen()
            {
                Content = parser.Parse(markupPath, new object())
            };

            SetupEvents();
        }

        private void SetupEvents()
        {
            var quitButton = Screen.FindControl<Button>("btnQuit");
            quitButton.Clicked += (sender, args) => System.Environment.Exit(0);

            var playButton = Screen.FindControl<Button>("btnPlay");
            playButton.Clicked += (sender, args) => _messageManager.SendMessage(EventType.UserInterface, Constants.MainMenu.PlayButtonAction, this, null);

#if DEBUG
            var spDebug = Screen.FindControl<StackPanel>("spDebug");
            spDebug.IsVisible = true;
#endif
        }
    }
}
