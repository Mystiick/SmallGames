using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownShooter.ECS.Managers;
using TopDownShooter.Managers;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using TopDownShooter.ECS;
using TopDownShooter.ECS.Components;
using MonoGame.Extended;
using TopDownShooter.ECS.Components.Templates;
using MonoGame.Extended.TextureAtlases;
using TopDownShooter.ECS.Engines;


namespace TopDownShooter.Stages
{
    public class PathfindingStage : BaseStage
    {
        private PlayerManager _player;
        private TileEngine _tileEngine;

        // TODO: Should these be somewhere else? A MapEngine of sorts?
        private TiledMapRenderer _mapRenderer;
        private TiledMap _map;

        public PathfindingStage() : base()
        {
            _player = new PlayerManager();
        }

        public override void LoadContent(ContentCacheManager contentManager)
        {
            base.LoadContent(contentManager);
            MessagingManager.Subscribe(EventType.LoadMap, LoadMap, this.StageID);

            ContentCache.LoadTileset(AssetName.Tileset);
            _player.SetSprite(ContentCache.GetClippedAsset(AssetName.Character_Brown_Idle));
        }

        public override void Start()
        {
            base.Start();
            MessagingManager.SendMessage(EventType.UserInterface, Constants.UserInterface.SetActive, this, ScreenName.Score);
            MessagingManager.SendMessage(EventType.Score, Constants.Score.PlayerScoreUpdated, this, 0);

            _player.InputManager = this.InputManager;
            _player.Camera = this.Camera;
            _tileEngine = this.EntityComponentManager.GetEngine<TileEngine>();
            
            // Setup Tile Map
            this.EntityComponentManager.AddEntity(
                new Entity(_tileEngine.BuildTileGrid(_map))
            );
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _player.Update(gameTime);

            CheckInputs(gameTime);

            // Lerp camera to player
            Vector2 cameraCenter = this.Camera.Position + this.Camera.Origin;
            this.Camera.LookAt(Vector2.Lerp(cameraCenter, _player.PlayerEntity.Transform.Position, 0.1f));

            MessagingManager.SendMessage(EventType.Score, Constants.Score.PlayerScoreUpdated, this, this.Camera.Zoom);

            this._mapRenderer.Update(gameTime);
        }

        private void CheckInputs(GameTime gameTime)
        {

        }

        public override void Draw()
        {
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            if (_map != null && _mapRenderer != null)
            {
                _mapRenderer.Draw(Camera.GetViewMatrix());
            }

            base.Draw();
        }

        private void LoadMap(object sender, object args)
        {
            // Load map
            _map = ContentCache.GetTiledMap(args.ToString());
            _mapRenderer = new TiledMapRenderer(GraphicsDevice);
            _mapRenderer.LoadMap(_map);

            InitNewMap();
        }

        private void InitNewMap()
        {
            // Clear current map
            EntityComponentManager.Clear();

            // Setup player
            SetupPlayerSpawn();

            // Add entities for collidable tiles
            CreateColliders();

            SpawnEnemies();
        }

        private void SetupPlayerSpawn()
        {
            var playerSpawn = _map.ObjectLayers.First(x => x.Name == Constants.TileMap.Layers.Spawners).Objects.First(x => x.Name == Constants.TileMap.PlayerSpawn);
            _player.PlayerEntity.Transform.Position = playerSpawn.Position - new Vector2(0, _map.TileHeight); // TODO: Need a better solution than this
            _player.SetWeapon(WeaponTemplates.Pistol(_player.PlayerEntity));

            EntityComponentManager.AddEntity(_player.PlayerEntity);
        }

        #region | Wall Colliders |
        private void CreateColliders()
        {
            var collidableLayers = _map.Layers.Where(x => x.Properties.FirstOrDefault(y => y.Key == Constants.TileMap.Properties.Collision).Value == "true");

            foreach (TiledMapTileLayer layer in collidableLayers)
            {
                ProcessLayer(layer);
            }
        }

        private void ProcessLayer(TiledMapTileLayer layer)
        {
            // This tracks if a tile has a collider that accounts for it already.
            // It is used to prevent smaller colliders from generating like: [ [ [ [ [ [ [ ]
            bool[,] colliderCreated = new bool[_map.Width, _map.Height];

            for (ushort y = 0; y < layer.Height; y++)
            {
                for (ushort x = 0; x < layer.Width; x++)
                {
                    ProcessTile(layer, x, y, colliderCreated);
                }
            }
        }

        private void ProcessTile(TiledMapTileLayer layer, ushort x, ushort y, bool[,] colliderCreated)
        {
            // Starting with the current tile, if it is a collidable tile (not blank), peek at the next horizontal tile
            // Continue peeking at the next horizontal tile down the line untiul we hit one that is blank.
            TiledMapTile firstTile = layer.GetTile(x, y);
            if (!firstTile.IsBlank && !colliderCreated[x, y])
            {
                TiledMapTile lastTile;

                // First look for a Horizontal grouping
                lastTile = GetLastTile(layer, firstTile, true);

                // Only create the group if it's not a standalone tile. Those will be created in the vertical grouping (this prevents overlaps and duplicate tiles)
                if (firstTile.GlobalIdentifier != lastTile.GlobalIdentifier)
                {
                    AddCollider(firstTile, lastTile, colliderCreated);
                }
                else
                {
                    // If a horizontal group doesn't exist, create a vertial group.
                    lastTile = GetLastTile(layer, firstTile, false);

                    // Don't care about GlobalIDs here, always create it. This lets 1x1 tiles generate a collider
                    AddCollider(firstTile, lastTile, colliderCreated);
                }
            }
        }

        private TiledMapTile GetLastTile(TiledMapTileLayer layer, TiledMapTile firstTile, bool horizontal)
        {
            TiledMapTile lastTile = firstTile;

            // Get next tile
            ushort x, y;
            x = lastTile.X;
            y = lastTile.Y;

            if (horizontal)
                x += 1;
            else
                y += 1;

            var success = layer.TryGetTile(x, y, out TiledMapTile? temp);

            if (success)
            {
                // TryGetLayer and GetLayer can return success = true and a layer that doesn't actually exist when trying to get a tile outside of bounds sometimes
                // If we've successfully gotten a tile, double check it's real
                if (horizontal && temp.Value.Y == lastTile.Y)
                {
                    return GetLastTile(layer, temp.Value, horizontal);
                }
                else if (!horizontal && temp.Value.X == lastTile.X)
                {
                    return GetLastTile(layer, temp.Value, horizontal);
                }
            }

            return lastTile;
        }

        private void AddCollider(TiledMapTile start, TiledMapTile end, bool[,] colliderCreated)
        {
            int sizeX = end.X - start.X + 1;
            int sizeY = end.Y - start.Y + 1;

            // Build out collider
            Vector2 location = new Vector2(start.X * _map.TileWidth, start.Y * _map.TileHeight);
            Point size = new Point(sizeX * _map.TileWidth, sizeY * _map.TileHeight);

            var entity = new Entity(new Component[] {
                new Transform() { Position = location },
                new BoxCollider() { BoundingBox = new Rectangle(Point.Zero, size) },
            })
            {
                Name = "Wall"
            };
            entity.Transform.TargetPosition = entity.Transform.Position;

            EntityComponentManager.AddEntity(entity);

            for (int x = start.X; x <= end.X; x++)
            {
                for (int y = start.Y; y <= end.Y; y++)
                {
                    colliderCreated[x, y] = true;
                }
            }
        }
        #endregion


        private void SpawnEnemies()
        {
            TiledMapObject[] enemySpawns = _map.ObjectLayers.FirstOrDefault(x => x.Name == Constants.TileMap.Layers.Enemies)?.Objects;

            if (enemySpawns != null)
            {
                foreach (TiledMapObject obj in enemySpawns)
                {
                    TextureRegion2D sprite = ContentCache.GetClippedAsset(AssetName.Character_Orange_Pistol);
                    Vector2 size = new Vector2(sprite.Width, sprite.Height);
                    EnemyType et = Enum.Parse<EnemyType>(obj.Properties.First(x => x.Key == "enemy_type").Value);

                    Entity e = new Entity();
                    e.AddComponents(new Component[] {
                        new Transform() { Position = obj.Position },
                        new Intelligence() { EnemyType = Enum.Parse<EnemyType>(obj.Properties.First(x => x.Key == "enemy_type").Value) },
                        new Health() { MaxHealth = 50 },
                        new Sprite() { Texture = sprite }, 
                        new BoxCollider() { BoundingBox = new Rectangle(0, 0, (int)size.X, (int)size.Y) },
                        new Velocity() { }
                    });
                    e.Name = $"Enemy-{et}";
                    e.Transform.Position -= new Vector2(0, _map.TileHeight); // TODO: Need a better solution than this

                    EntityComponentManager.AddEntity(e);
                }
            }
        }
    }
}
