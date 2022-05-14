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
    public class ScoreScreen : BaseScreen
    {
        private readonly Guid _parentGuid;

        public ScoreScreen() : base("UI\\Score.xml")
        {
            _parentGuid = Guid.NewGuid();
        }

        protected override void SetupEvents()
        {
            MessagingService.Subscribe(
                EventType.Score,
                Constants.Score.PlayerScoreUpdated,
                (sender, args) =>
                {
                    Screen.FindControl<Label>("lblScore").Content = $"Score: {args}";
                },
                _parentGuid
            );

            MessagingService.Subscribe(
                EventType.Score,
                Constants.Score.EnemyCountUpdated,
                (sender, args) =>
                {
                    Screen.FindControl<Label>("lblEnemies").Content = $"Enemies Remaining: {args}";
                },
                _parentGuid
            );
        }
    }
}
