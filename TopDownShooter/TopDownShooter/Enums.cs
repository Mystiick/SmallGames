using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownShooter
{
    public enum StageType
    {
        None,
        MainMenu,
        Debug
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

    public enum EnemyType
    {
        None,
        Dummy,
        Turret,
        Follower
    }

    public enum WeaponType
    {
        Single,
        Automatic,
        Shotgun
    }
}
