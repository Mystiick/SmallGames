using Microsoft.Xna.Framework;

namespace TopDownShooter
{
    public struct Constants
    {
        public struct MainMenu
        {
            public const string PlayButtonAction = "MainMenu.PlayButton";
            public const string PathfinderButtonAction = "MainMenu.Pathfinder";
        }

        public struct UserInterface
        {
            public const string SetActive = "UI.SetActive";
        }

        public struct Score
        {
            public const string PlayerScoreUpdated = "Score.PlayerScoreUpdated";
            public const string EnemyCountUpdated = "Score.EnemyCountUpdated";
        }

        public struct GameEvent
        {
            public const string EnemyKilled = "GameEvent.EnemyKilled";
            public const string PlayerKilled = "GameEvent.PlayerKilled";
            public const string MapGridReset = "GameEvent.MapGridReset";
            public const string SetupWorld = "LoadMap.SetupWorld";
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
            public const string Wall = "Wall";

            public const string Bullet = "Bullet";
            public const float BulletBaseSpeed = 500f;
            public static readonly Rectangle BulletCollider = new Rectangle(1, 0, 1, 1);
        }

    }
}
