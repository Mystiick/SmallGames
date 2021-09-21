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

    public enum AssetName
    {
        Tileset,
        Character_Brown_Idle, Character_Orange_Pistol,
        Grass1, Grass2, Grass3, Grass4,
        Dirt1, Dirt2,
        Gravel1, Gravel2, Gravel3, Gravel4,
        Bullet
    }

    public enum ScreenName
    {
        None,
        MainMenu,
        Score
    }

    public enum EventType
    {
        Test,
        UserInterface,
        Score,
        LoadMap,
        Spawn,
        GameEvent
    }

    public enum KeyBinding
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Shoot,
        ZoomIn,
        ZoomOut,
        Debug,
        WeaponOne,
        WeaponTwo,
        WeaponThree,
    }

    public enum MouseAndKeys
    {
        LeftClick,
        RightClick,
        ScrollClick,
        ScrollUp,
        ScrollDown,
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
        Space,
        Alt,
        Shift,
        Tab,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Zero,
        Tilde
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

    public enum EntityType 
    {
        Unassigned,
        Player,
        Wall,
        Enemy,
        Pickup,
    }
}
