using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Markup;
using TopDownShooter.Managers;

namespace TopDownShooter.UI
{
    public class ScoreScreen : IScreen
    {
        private readonly string markupPath = "UI\\Score.xml";
        private readonly MessagingManager _messageManager;
        private readonly Guid _parentGuid;

        public Screen Screen { get; private set; }

        public ScoreScreen(MessagingManager messagingManager)
        {
            _messageManager = messagingManager;
            _parentGuid = Guid.NewGuid();
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
            _messageManager.Subscribe(
                EventType.Score, 
                Constants.Score.PlayerScoreUpdated, 
                (s, a) => {
                    Screen.FindControl<Label>("lblScore").Content = $"Score: {a}";
                }, 
                _parentGuid
            );
        }
    }
}
