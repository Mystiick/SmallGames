using Microsoft.Xna.Framework;

using MystiickCore.Models;

namespace TopDownShooter
{
    public class Constants
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

        public static SpriteAtlas[] SpriteAtlas = new SpriteAtlas[]
        {
            new SpriteAtlas() { Name = AssetName.Character_Brown_Idle, Position = new Point(479, 2), Size = new Point(8, 12) },
            new SpriteAtlas() { Name = AssetName.Character_Orange_Pistol, Position = new Point(513, 36), Size = new Point(12, 12) },
            new SpriteAtlas() { Name = AssetName.Grass1, Position = new Point(0, 0), Size = new Point(16, 16) },
            new SpriteAtlas() { Name = AssetName.Grass2, Position = new Point(17, 0), Size = new Point(16, 16) },
            new SpriteAtlas() { Name = AssetName.Grass3, Position = new Point(34, 0), Size = new Point(16, 16) },
            new SpriteAtlas() { Name = AssetName.Grass4, Position = new Point(41, 0), Size = new Point(16, 16) },
            new SpriteAtlas() { Name = AssetName.Bullet, Position = new Point(529, 279), Size = new Point(3, 1) }
        };
    }

}
