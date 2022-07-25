using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using MonoGame.Extended;

using MystiickCore;
using MystiickCore.ECS;
using MystiickCore.ECS.Components;
using MystiickCore.ECS.Engines;

namespace TopDownShooter.Intelligences
{
    public class Follower : ShooterIntelligence
    {

        Queue<Tile> pathToPlayer;
        public float MovementSpeed = 50f;

        /// <summary>Number of seconds where the entity can still "see" the player after it loses sight</summary>
        public float CheatVision = 2f;
        private float _cheatVisionCooldown;

        public Follower()
        {
            _cheatVisionCooldown = CheatVision;
        }

        public override void Update(GameTime gameTime, List<Entity> allEntities)
        {
            base.Update(gameTime, allEntities);
            _cheatVisionCooldown += gameTime.GetElapsedSeconds();

            float distanceToPlayer = Vector2.Distance(CurrentEntity.Transform.Position, PlayerEntity.Transform.Position);

            if (this.EntityCanSeePlayer)
            {
                _cheatVisionCooldown = 0f;

                // Only move the entity if it is over 50 units away from the player
                if (distanceToPlayer > 50)
                {
                    MoveTowardPlayer();
                }
            }
            else if (pathToPlayer != null && pathToPlayer.Count > 0)
            {
                Tile myTile = GetMyTile();

                // We can't see the player, but can remember where they were, try moving there
                MoveTowardTile(pathToPlayer.Peek(), myTile);

                if (pathToPlayer.Peek().ID == myTile.ID)
                {
                    pathToPlayer.Dequeue();
                }
            }
        }

        public override void PlayerInformationChanged()
        {
            base.PlayerInformationChanged();

            if (EntityCanSeePlayer || _cheatVisionCooldown < CheatVision)
            {
                // Only recreate the queue if the entity can see the player, otherwise it doesn't do anything
                pathToPlayer = new Queue<Tile>();

                Tile temp = GetMyTile();
                while (temp.DistanceToPlayer > 0)
                {
                    // Find the next tile and add it to the queue
                    temp = temp.Neighbors.Where(x => x.CanTravelThrough).OrderBy(x => x.DistanceToPlayer).FirstOrDefault();
                    pathToPlayer.Enqueue(temp);
                }
            }
        }

        private void MoveTowardPlayer()
        {
            // Find my current Tile
            Tile myTile = GetMyTile();
            Tile targetTile = myTile;

            foreach (Tile t in myTile.Neighbors)
            {
                if (t.DistanceToPlayer < targetTile.DistanceToPlayer && t.CanTravelThrough)
                {
                    targetTile = t;
                }
            }

            MoveTowardTile(targetTile, myTile);
        }

        private Tile GetMyTile()
        {
            Point p = new Point(
                (int)(CurrentEntity.Transform.Position.X / Grid.TileWidth),
                (int)(CurrentEntity.Transform.Position.Y / Grid.TileHeight)
            );

            return Grid.Tiles[p.X, p.Y];
        }

        private void MoveTowardTile(Tile targetTile, Tile sourceTile)
        {
            Vector2 targetPosition = Helpers.DetermineTilePosition(targetTile, Grid);
            Vector2 sourcePosition = Helpers.DetermineTilePosition(sourceTile, Grid);

            // First, try casting a ray to see if we can get to the location. If the entity cannot reach it, it's probably getting hitched up on a corner and needs to use NSEW instead of all angles
            var collidables = PhysicsEngine.CastAllToward(sourcePosition, targetPosition, this.AllEntities);
            if (collidables.Any(x => x.ID != this.CurrentEntity.ID))
            {
                // There is something in the way, try navigating to source tile via NSEW instead of diagonally
                Tile newTarget = new Tile[] { sourceTile.North, sourceTile.South, sourceTile.East, sourceTile.West }
                    .OrderBy(x => x.DistanceToPlayer)
                    .FirstOrDefault(x => x.CanTravelThrough);

                System.Diagnostics.Debug.Assert(newTarget != null, "This should never happen, but just to be sure, here's a hard break");

                targetPosition = Helpers.DetermineTilePosition(newTarget, Grid);
            }

            // Need to subtract origin here, otherwise it tries to line up the top left corner with the center of the tile, causing it to get stuck on 1x1 gaps or corners
            EntityVelocity.Direction = targetPosition - CurrentEntity.Transform.Position - CurrentEntity.Sprite.Origin;
            EntityVelocity.Speed = MovementSpeed;
        }
    }
}
