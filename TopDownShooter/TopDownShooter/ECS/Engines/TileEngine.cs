using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using TopDownShooter.ECS.Components;

namespace TopDownShooter.ECS.Engines
{
    public class TileEngine : Engine
    {
        private Point _lastCalculatedPlayerPosition;

        public override Type[] RequiredComponents => new[] { typeof(TileGrid) };

        public override void Update(GameTime gameTime, List<Entity> allEntities)
        {
            var player = allEntities.FirstOrDefault(x => x.Name == Constants.Entities.Player);

            // Should only be one, but loop throug it anyway
            for (int i = 0; i < this.Entities.Count; i++)
            {
                // TODO: Check allEntities if player has moved significantally (X number of units)
                // If so, recalculate distance values
                var grid = this.Entities[i].GetComponent<TileGrid>();

                if (_lastCalculatedPlayerPosition == Point.Zero || _lastCalculatedPlayerPosition != ConvertToTilePosition(grid, player.Transform))
                {
                    SetWeights(grid, player);
                }
            }
        }

        public TileGrid BuildTileGrid(TiledMap map)
        {
            // Reset player's calculated position
            _lastCalculatedPlayerPosition = Point.Zero;

            // Build out entire grid
            var output = new TileGrid()
            {
                Tiles = new Tile[map.Width, map.Height],
                TileWidth = map.TileWidth,
                TileHeight = map.TileHeight
            };

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Height; x++)
                {
                    output.Tiles[x, y] = new Tile()
                    {
                        DistanceToPlayer = -1,
                        Visited = false,
                        CanTravelThrough = true
                    };
                }
            }

            // Mark anything that is collidable as CanTravelThrough = false
            TiledMapTileLayer[] collidableLayers = map.Layers.Where(x => x.Properties.FirstOrDefault(y => y.Key == Constants.TileMap.Properties.Collision).Value == "true").Cast<TiledMapTileLayer>().ToArray();

            for (int i = 0; i < collidableLayers.Length; i++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    for (int x = 0; x < map.Height; x++)
                    {
                        var tile = output.Tiles[x, y];
                        // Set neighbors
                        if (y > 0)
                        {
                            tile.North = output.Tiles[x, y - 1];
                        }
                        if (y < output.Tiles.GetUpperBound(1))
                        {
                            tile.South = output.Tiles[x, y + 1];
                        }
                        if (x > 0)
                        {
                            tile.West = output.Tiles[x - 1, y];
                        }
                        if (x < output.Tiles.GetUpperBound(0))
                        {
                            tile.East = output.Tiles[x + 1, y];
                        }

                        // Set colliders
                        if (!collidableLayers[i].GetTile((ushort)x, (ushort)y).IsBlank)
                        {
                            tile.CanTravelThrough = false;
                        }
                    }
                }
            }

            return output;
        }

        private void SetWeights(TileGrid grid, Entity target)
        {
            // First, get the starting tile
            var startPosition = ConvertToTilePosition(grid, target.Transform);
            _lastCalculatedPlayerPosition = startPosition;

            grid.Reset();

            var startingTile = grid.Tiles[startPosition.X, startPosition.Y];
            var tileQueue = new Queue<Tile>();

            ProcessTile(startingTile, tileQueue, -1);

            while (tileQueue.Count > 0)
            {
                Tile temp = tileQueue.Dequeue();
                ProcessNeighbors(temp, tileQueue, temp.DistanceToPlayer);
            }

#if DEBUG
            grid.PrintDebugGrid();
#endif
        }

        private void ProcessNeighbors(Tile currentTile, Queue<Tile> tiles, int distance)
        {
            ProcessTile(currentTile.North, tiles, distance);
            ProcessTile(currentTile.South, tiles, distance);
            ProcessTile(currentTile.East, tiles, distance);
            ProcessTile(currentTile.West, tiles, distance);
        }

        private void ProcessTile(Tile currentTile, Queue<Tile> tiles, int distance)
        {
            if (currentTile != null && !currentTile.Visited && currentTile.CanTravelThrough)
            {
                currentTile.DistanceToPlayer = distance + 1;
                currentTile.Visited = true;
                tiles.Enqueue(currentTile);
            }
        }

        private Point ConvertToTilePosition(TileGrid grid, Transform t)
        {
            int x = (int)t.Position.X / grid.TileWidth;
            int y = (int)t.Position.Y / grid.TileHeight;

            return new Point(x, y);
        }
    }
}
