using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Gui;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Text;
using TopDownShooter.Managers;
using TopDownShooter.Services;

namespace TopDownShooter.Stages
{
    public class MainMenu : BaseStage
    {

        public MainMenu() : base()
        {

        }

        public override void Start()
        {
            base.Start();

            MessagingService.Subscribe(EventType.UserInterface, Constants.MainMenu.PlayButtonAction, Play_Click, this.StageID);
            MessagingService.Subscribe(EventType.UserInterface, Constants.MainMenu.PathfinderButtonAction, Pathfinder_Click, this.StageID);
        }

        public override void LoadContent(ContentCacheManager contentManager)
        {
            base.LoadContent(contentManager);
        }

        public override void Draw()
        {
            base.Draw();
        }

        private void Play_Click(object sender, object args)
        {
            StageManager.SetNextStage<WorldStage>();
            MessagingService.SendMessage(EventType.LoadMap, "setup_world", this, "MiniComplex");
        }

        private void Pathfinder_Click(object sender, object args)
        {
            StageManager.SetNextStage<WorldStage>();
            MessagingService.SendMessage(EventType.LoadMap, "setup_world", this, "Pathfinder");
        }
    }
}
