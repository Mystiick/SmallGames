using System;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

using MystiickCore;
using MystiickCore.Services;
using MystiickCore.Models;

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
