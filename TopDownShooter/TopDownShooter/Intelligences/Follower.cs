using System;       
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TopDownShooter.ECS;
using TopDownShooter.ECS.Components;
using TopDownShooter.ECS.Engines;
using TopDownShooter.Managers;

namespace TopDownShooter.Intelligences
{
    public class Follower : IImplementation
    {
        public Entity CurrentEntity { get; set; }
        public Entity PlayerEntity { get; set; }
        public TileGrid Grid { get; set; }

        public void Update(GameTime gameTime, List<Entity> allEntities)
        {
            float distanceToPlayer = Vector2.Distance(CurrentEntity.Transform.Position, PlayerEntity.Transform.Position); 
            var v = CurrentEntity.GetComponent<Velocity>();

            if (v != null)
            {
                // Reset here, because it will get set any time the entity needs to move
                v.Speed = 0;

                if (CanSeePlayer(allEntities))
                {
                    // Rotate entity to face player
                    CurrentEntity.Transform.Rotation = Helpers.DetermineAngle(CurrentEntity.Transform.Position, PlayerEntity.Transform.Position);

                    // Only move the entity if it is over 50 units away from the player
                    if (distanceToPlayer > 50)
                    {
                        MoveTowardPlayer(v);
                    }

                    // If the player is closer than 100 units, shoot at them
                    if (distanceToPlayer < 100)
                    {
                        ShootAtPlayer();
                    }
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

        private bool CanSeePlayer(List<Entity> allEntities)
        {
            // Get distance between NPC and player
            float distance = Vector2.Distance(CurrentEntity.Transform.Position, PlayerEntity.Transform.Position);
            Vector2 direction = PlayerEntity.Transform.Position - CurrentEntity.Transform.Position;
            direction.Normalize();

            Entity[] collidedEntities;

            // Shoot a ray toward the player
            collidedEntities = PhysicsEngine.CastAll(CurrentEntity.Transform.Position, direction, distance, allEntities);

            // If there are any "Wall" colliders hit, the NPC cannot see the player
            foreach (Entity e in collidedEntities)
            {
                if (e.Name == "Wall")
                {
                    return false;
                }
            }
            
            // Nothing is in the way, the NPC can see the player
            return true;
        }

        private void ShootAtPlayer()
        {
            // TODO: 61
        }
    }
}