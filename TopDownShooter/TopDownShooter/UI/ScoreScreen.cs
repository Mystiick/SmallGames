using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Markup;
using TopDownShooter.Managers;
using TopDownShooter.Services;

namespace TopDownShooter.UI
{
    public class ScoreScreen : IScreen
    {
        private readonly string markupPath = "UI\\Score.xml";
        private readonly Guid _parentGuid;

        public Screen Screen { get; private set; }

        public ScoreScreen()
        {
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
            MessagingService.Subscribe(
                EventType.Score, 
                Constants.Score.PlayerScoreUpdated, 
                (sender, args) => {
                    Screen.FindControl<Label>("lblScore").Content = $"Score: {args}";
                }, 
                _parentGuid
            );

            MessagingService.Subscribe(
                EventType.Score,
                Constants.Score.EnemyCountUpdated,
                (sender, args) => {
                    Screen.FindControl<Label>("lblEnemies").Content = $"Enemies Remaining: {args}";
                },
                _parentGuid
            );
        }
    }
}
