using Microsoft.Xna.Framework;

namespace MystiickCore.ECS.Components;

public class TileGrid : Component
{
    public Tile[,] Tiles { get; set; }
    public int TileWidth { get; set; }
    public int TileHeight { get; set; }
    public Point MapSize { get; set; }

    public void Reset()
    {
        foreach (Tile t in Tiles)
        {
            t.Reset();
        }
    }

#if DEBUG
    #region | DEBUG |
    public void PrintDebugGrid()
    {
        if (Debugger.ShowDebugInfo)
        {
            Console.WriteLine("");

            for (int y = 0; y <= Tiles.GetUpperBound(1); y++)
            {
                for (int x = 0; x <= Tiles.GetUpperBound(0); x++)
                {
                    var tile = Tiles[x, y];

                    if (!tile.CanTravelThrough)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else if (tile.DistanceToPlayer == -1)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }

                    Console.Write(tile.DebugString());
                    Console.ResetColor();
                }

                Console.WriteLine("");
            }
        }
    }
    #endregion
#endif

}

public class Tile
{
    public int DistanceToPlayer { get; set; }
    public bool Visited { get; set; }
    public bool CanTravelThrough { get; set; } // If false, it is a collider, and should be ignored
    public Point Location { get; set; }

    public Tile North { get; set; }
    public Tile South { get; set; }
    public Tile East { get; set; }
    public Tile West { get; set; }
    public Tile[] Neighbors { get; set; }

    public Guid ID { get; private set; } = Guid.NewGuid();

    private const int default_distance = -1;

#if DEBUG
    public string DebugString()
    {
        return string.Format("{0,3}", this.CanTravelThrough ? DistanceToPlayer.ToString() : "*");
    }
#endif

    public void Reset()
    {
        DistanceToPlayer = default_distance;
        Visited = false;
    }
}
