namespace TopDownShooter
{
    public struct Constants
    {
        public struct MainMenu
        {
            public const string PlayButtonAction = "MainMenu.PlayButton";
        }

        public struct UserInterface
        {
            public const string SetActive = "UI.SetActive";
        }

        public struct Score
        {
            public const string PlayerScoreUpdated = "Score.PlayerScoreUpdated";
        }

        public struct TileMap
        {
            public const string PlayerSpawn = "PlayerSpawn";

            public struct Layers
            {
                public const string Spawners = "Spawners";
                public const string Enemies = "Enemies";
            }

            public struct Properties
            {
                public const string Collision = "collision";
                public const string EnemyType = "enemy_type";
            }
        }

        public struct Entities
        {
            public const string Player = "Player";
        }

    }
}
