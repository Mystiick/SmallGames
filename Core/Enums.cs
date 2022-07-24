using System;
using System.Collections.Generic;
using System.Text;

namespace MystiickCore;

// TODO: Probably can just get rid of this and use constants for messages types
public enum EventType
{
    Test,
    UserInterface,
    Score,
    LoadMap,
    Spawn,
    GameEvent
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

// TODO: Is there a way to get rid of this
public enum EntityType
{
    Unassigned,
    Player,
    Wall,
    Enemy,
    Pickup,
    TileGrid,
}
