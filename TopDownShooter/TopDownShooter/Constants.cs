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

    public struct AssetName
    {
        public const string Tileset = nameof(Tileset);
        public const string Character_Brown_Idle = nameof(Character_Brown_Idle);
        public const string Character_Orange_Pistol = nameof(Character_Orange_Pistol);
        public const string Grass1 = nameof(Grass1);
        public const string Grass2 = nameof(Grass2);
        public const string Grass3 = nameof(Grass3);
        public const string Grass4 = nameof(Grass4);
        public const string Dirt1 = nameof(Dirt1);
        public const string Dirt2 = nameof(Dirt2);
        public const string Gravel1 = nameof(Gravel1);
        public const string Gravel2 = nameof(Gravel2);
        public const string Gravel3 = nameof(Gravel3);
        public const string Gravel4 = nameof(Gravel4);
        public const string Bullet = nameof(Bullet);
    }

    public struct ScreenName
    {
        public const string None = nameof(None);
        public const string MainMenu = nameof(MainMenu);
        public const string Score = nameof(Score);
    }

    public struct KeyBinding
    {
        public const string MoveUp = nameof(MoveUp);
        public const string MoveDown = nameof(MoveDown);
        public const string MoveLeft = nameof(MoveLeft);
        public const string MoveRight = nameof(MoveRight);
        public const string Shoot = nameof(Shoot);
        public const string ZoomIn = nameof(ZoomIn);
        public const string ZoomOut = nameof(ZoomOut);
        public const string Debug = nameof(Debug);
        public const string WeaponOne = nameof(WeaponOne);
        public const string WeaponTwo = nameof(WeaponTwo);
        public const string WeaponThree = nameof(WeaponThree);
    }

}
