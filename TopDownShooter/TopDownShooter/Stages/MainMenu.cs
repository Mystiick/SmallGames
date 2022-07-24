using MystiickCore;
using MystiickCore.Services;
using MystiickCore.Models;
using MystiickCore.Managers;

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
            MessagingService.SendMessage(EventType.LoadMap, MystiickCore.Constants.GameEvent.SetupWorld, this, "MiniComplex");
        }

        private void Pathfinder_Click(object sender, object args)
        {
            StageManager.SetNextStage<WorldStage>();
            MessagingService.SendMessage(EventType.LoadMap, MystiickCore.Constants.GameEvent.SetupWorld, this, "Pathfinder");
        }
    }
}
