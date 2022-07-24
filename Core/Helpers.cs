using Microsoft.Xna.Framework;

using MystiickCore.ECS.Components;

namespace MystiickCore;

public static class Helpers
{
    public static float DetermineAngle(Vector2 v1, Vector2 v2)
    {
        return (float)Math.Atan2(v2.Y - v1.Y, v2.X - v1.X);
    }

    public static Vector2 DetermineTilePosition(Tile t, TileGrid g)
    {
        // Adding half the tile width,height to the X,Y coords lets us line up the center of the sprite with the center of the tile
        return new Vector2(t.Location.X * g.TileWidth + (g.TileWidth / 2), t.Location.Y * g.TileHeight + (g.TileHeight / 2));
    }
}
