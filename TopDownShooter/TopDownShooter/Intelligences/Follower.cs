using System;       
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

using MonoGame.Extended;

using TopDownShooter.ECS;
using TopDownShooter.ECS.Components;
using TopDownShooter.ECS.Components.Templates;
using TopDownShooter.ECS.Engines;
using TopDownShooter.Managers;
using TopDownShooter.Services;

namespace TopDownShooter.Intelligences
{
    public class Follower : BaseIntelligence
    {
        public override void Update(GameTime gameTime, List<Entity> allEntities)
        {
            base.Update(gameTime, allEntities);

            var v = CurrentEntity.GetComponent<Velocity>();
            float distanceToPlayer = Vector2.Distance(CurrentEntity.Transform.Position, PlayerEntity.Transform.Position); 
            
            if (CanSeePlayer(allEntities))
            {
                // Only move the entity if it is over 50 units away from the player
                if (distanceToPlayer > 50)
                {
                    MoveTowardPlayer(v);
                }
            }
        }

        private void MoveTowardPlayer(Velocity v)
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

            // Adding half the tile width,height to the X,Y coords lets us line up the center of the sprite with the center of the tile
            Vector2 targetPosition = new Vector2(targetTile.Location.X * Grid.TileWidth + (Grid.TileWidth / 2), targetTile.Location.Y * Grid.TileHeight + (Grid.TileHeight / 2));

            // Need to subtract origin here, otherwise it tries to line up the top left corner with the center of the tile, causing it to get stuck on 1x1 gaps or corners
            v.Direction = targetPosition - CurrentEntity.Transform.Position - CurrentEntity.Sprite.Origin;
            v.Speed = 50;
        }
    }
}