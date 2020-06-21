using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            base.Update(gameTime, allEntities);

            var player = allEntities.FirstOrDefault(x => x.Name == Constants.Entities.Player);

            // Should only be one, but loop throug it anyway
            for (int i = 0; i < this.Entities.Count; i++)
            {
                var grid = this.Entities[i].GetComponent<TileGrid>();

                // If the player has moved to a different tile, reset the weights
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
                TileHeight = map.TileHeight,
                MapSize = new Point(map.Width, map.Height)
            };

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Height; x++)
                {
                    output.Tiles[x, y] = new Tile()
                    {
                        DistanceToPlayer = -1,
                        Visited = false,
                        CanTravelThrough = true,
                        Location = new Point(x, y) //Location = new Point(x * output.TileWidth, y * output.TileHeight)
                    };
                }
            }

            // Mark anything that is collidable as CanTravelThrough = false
            TiledMapTileLayer[] collidableLayers = map.Layers.Where(x => x.Properties.FirstOrDefault(y => y.Key == Constants.TileMap.Properties.Collision).Value == "true").Cast<TiledMapTileLayer>().ToArray();

            for (int i = 0; i < collidableLayers.Length; i++)
            {
                for (int y = 0; y < output.MapSize.Y; y++)
                {
                    for (int x = 0; x < output.MapSize.Y; x++)
                    {
                        var tile = output.Tiles[x, y];
                        List<Tile> temp = new List<Tile>();

                        bool hasNorth, hasSouth, hasEast, hasWest;
                        hasNorth = y > 0;
                        hasSouth = y < output.MapSize.Y - 1;
                        hasEast = x < output.MapSize.X - 1;
                        hasWest = x > 0;

                        // Set neighbors
                        if (hasNorth)
                        {
                            tile.North = output.Tiles[x, y - 1];
                            temp.Add(tile.North);

                            if (hasWest)
                            {
                                temp.Add(output.Tiles[x - 1, y - 1]);
                            }
                            if (hasEast)
                            {
                                temp.Add(output.Tiles[x + 1, y - 1]);
                            }
                        }
                        if (hasSouth)
                        {
                            tile.South = output.Tiles[x, y + 1];
                            temp.Add(tile.South);

                            if (hasWest)
                            {
                                temp.Add(output.Tiles[x - 1, y + 1]);
                            }
                            if (hasEast)
                            {
                                temp.Add(output.Tiles[x + 1, y + 1]);
                            }
                        }
                        if (hasWest)
                        {
                            tile.West = output.Tiles[x - 1, y];
                            temp.Add(tile.West);
                        }
                        if (hasEast)
                        {
                            tile.East = output.Tiles[x + 1, y];
                            temp.Add(tile.East);
                        }
                        tile.Neighbors = temp.ToArray();

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
            var tileQueue = new Queue<Tile>();
            Point startPosition = ConvertToTilePosition(grid, target.Transform);
            Tile startingTile = grid.Tiles[startPosition.X, startPosition.Y];
            
            // Keep track of where the player was last, so we don't need to do this every frame
            _lastCalculatedPlayerPosition = startPosition;

            grid.Reset();

            startingTile.DistanceToPlayer = 0;
            startingTile.Visited = true;
            tileQueue.Enqueue(startingTile);

            Tile[] adjacentTiles;
            while (tileQueue.Count > 0)
            {
                Tile tile = tileQueue.Dequeue();
                adjacentTiles = new[] { tile.North, tile.South, tile.East, tile.West };

                // Process any neighbors that have not been visited yet
                for (int i = 0; i < adjacentTiles.Length; i++)
                {
                    Tile neighbor = adjacentTiles[i];
                    bool addToQueue = ProcessNeighbor(neighbor, tile.DistanceToPlayer);

                    if (addToQueue)
                    {
                        tileQueue.Enqueue(neighbor);
                    }
                }
            }
        }

        private bool ProcessNeighbor(Tile neighbor, int distance)
        {
            if (neighbor != null && !neighbor.Visited && neighbor.CanTravelThrough)
            {
                neighbor.DistanceToPlayer = distance + 1;
                neighbor.Visited = true;
                return true;
            }
            else
            {
                return false;
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
