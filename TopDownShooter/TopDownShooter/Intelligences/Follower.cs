using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TopDownShooter.ECS;
using TopDownShooter.ECS.Components;

namespace TopDownShooter.Intelligences
{
    public class Follower : IImplementation
    {
        public Entity CurrentEntity { get; set; }
        public Entity PlayerEntity { get; set; }
        public TileGrid Grid { get; set; }

        public void Update(GameTime gameTime, List<Entity> allEntities)
        {
            var v = CurrentEntity.GetComponent<Velocity>();
            if (v != null)
            {
                // Find my current Tile
                Point p = new Point(
                    (int)(CurrentEntity.Transform.Position.X / Grid.TileWidth),
                    (int)(CurrentEntity.Transform.Position.Y / Grid.TileHeight)
                );
                Tile myTile = Grid.Tiles[p.X, p.Y];

                Tile targetTile = myTile;

                foreach (Tile t in myTile.Neighbors)
                {
                    if (t.DistanceToPlayer < targetTile.DistanceToPlayer && t.CanTravelThrough)
                    {
                        targetTile = t;
                    }
                }

                Vector2 targetPosition = new Vector2(targetTile.Location.X * Grid.TileWidth + (Grid.TileWidth / 2), targetTile.Location.Y * Grid.TileHeight + (Grid.TileHeight / 2));

                v.Direction = targetPosition - CurrentEntity.Transform.Position;
                v.Speed = 50;
            }
        }
    }
}
