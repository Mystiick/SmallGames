using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;

namespace TopDownShooter.ECS.Components
{
    public class TileGrid : Component
    {
        public Tile[,] Tiles;
        public int TileWidth;
        public int TileHeight;
        public Point MapSize;

        public void Reset()
        {
            foreach (Tile t in Tiles)
            {
                t.Reset();
            }
        }

#if DEBUG
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
#endif

    }

    public class Tile
    {
        public int DistanceToPlayer;
        public bool Visited;
        public bool CanTravelThrough; // If false, it is a collider, and should be ignored
        public Point Location;

        public Tile North;
        public Tile South;
        public Tile East;
        public Tile West;
        public Tile[] Neighbors;

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
}
