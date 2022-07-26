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
