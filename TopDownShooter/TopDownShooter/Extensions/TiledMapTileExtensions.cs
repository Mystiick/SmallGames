using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;

namespace TopDownShooter.Extensions.Tiled
{
    public static class TiledMapTileExtensions
    {
        public static Point ToPoint(this TiledMapTile input) => new Point(input.X, input.Y);
    }
}