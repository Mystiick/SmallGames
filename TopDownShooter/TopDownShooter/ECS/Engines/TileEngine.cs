using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;

using MystiickCore;
using MystiickCore.ECS;
using MystiickCore.ECS.Components;
using MystiickCore.Services;

namespace TopDownShooter.ECS.Engines
{
    public class TileEngine : Engine
    {
        private Point _lastCalculatedPlayerPosition;

        public override Type[] RequiredComponents => new[] { typeof(TileGrid) };

        public override void Update(GameTime gameTime, List<Entity> allEntities)
        {
            base.Update(gameTime, allEntities);

            var player = allEntities.FirstOrDefault(x => x.Type == EntityType.Player);

            if (player != null)
            {
                // Should only be one, but loop throug it anyway
                for (int i = 0; i < this.Entities.Count; i++)
                {
                    var grid = this.Entities[i].GetComponent<TileGrid>();
                    Point playerPosition = ConvertToTilePosition(grid, player.Transform);

                    // If the player hasn't been calculated or moved to a different tile, reset the weights
                    if ((_lastCalculatedPlayerPosition == Point.Zero || _lastCalculatedPlayerPosition != playerPosition) && CanCalculateWeights(grid, playerPosition))
                    {
                        SetWeights(grid, playerPosition);
                        MessagingService.SendMessage(EventType.GameEvent, MystiickCore.Constants.GameEvent.MapGridReset, this);
                    }

                    // Keep track of where the player was last, so we don't need to do this every frame
                    _lastCalculatedPlayerPosition = playerPosition;
                }
            }
        }

        public bool CanCalculateWeights(TileGrid grid, Point startPosition)
        {
            // Validate that the player is on the grid. This should never realistically happen but is a safeguard
            if (grid.Tiles.GetLength(0) <= startPosition.X || startPosition.X < 0)
            {
                Console.WriteLine("X Outside of grid, not calculating anything");
                return false;
            }
            else if (grid.Tiles.GetLength(1) <= startPosition.Y || startPosition.Y < 0)
            {
                Console.WriteLine("Y Outside of grid, not calculating anything");
                return false;
            }

            return true;
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

        // TODO: Decouple from tiled, and move into a Core engine
        private void SetWeights(TileGrid grid, Point startPosition)
        {
            var tileQueue = new Queue<Tile>();
            Tile startingTile = grid.Tiles[startPosition.X, startPosition.Y];

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
