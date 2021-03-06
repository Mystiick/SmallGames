using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.TextureAtlases;

using MystiickCore;
using MystiickCore.ECS;
using MystiickCore.ECS.Components;
using MystiickCore.Models;
using MystiickCore.Services;
using MystiickCore.Managers;

using TopDownShooter.ECS.Components;
using TopDownShooter.ECS.Components.Templates;
using TopDownShooter.ECS.Engines;
using TopDownShooter.Managers;
using TopDownShooter.Extensions.Tiled;

namespace TopDownShooter.Stages;

/// <summary>
/// Primary Stage that manages loading, swapping, and playing of levels
/// </summary>
public class WorldStage : BaseStage
{
    private PlayerManager _player;
    private WeaponManager _weaponManager;
    private TileEngine _tileEngine;

    // TODO: Should these be somewhere else? A MapEngine of sorts?
    private TiledMapRenderer _mapRenderer;
    private TiledMap _map;

    public WorldStage() : base()
    {
        _player = new PlayerManager();
    }

    public override void LoadContent(ContentCacheManager contentManager)
    {
        base.LoadContent(contentManager);
        MessagingService.Subscribe(EventType.LoadMap, LoadMap, this.StageID);

        ContentCache.LoadTileset(AssetName.Tileset, Constants.SpriteAtlas);
        _player.SetSprite(ContentCache.GetClippedAsset(AssetName.Character_Brown_Idle));
        _weaponManager = new WeaponManager(contentManager);
    }

    public override void Start()
    {
        base.Start();
        MessagingService.SendMessage(EventType.UserInterface, Constants.UserInterface.SetActive, this, ScreenName.Score);
        MessagingService.SendMessage(EventType.Score, Constants.Score.PlayerScoreUpdated, this, 0);
        var enemyCount = this.EntityComponentManager.GetEngine<IntelligenceEngine>().MyEntities.Count(x => x.Type == EntityType.Enemy);
        MessagingService.SendMessage(EventType.Score, Constants.Score.EnemyCountUpdated, this, enemyCount);

        MessagingService.Subscribe(EventType.GameEvent, MystiickCore.Constants.GameEvent.PlayerKilled, (s, a) => { ResetMap(); }, this.StageID);

        _player.InputManager = this.InputManager;
        _player.Camera = this.Camera;

    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _player.Update(gameTime);

        CheckInputs(gameTime);

        // Lerp camera to player
        Vector2 cameraCenter = this.Camera.Position + this.Camera.Origin;
        this.Camera.LookAt(Vector2.Lerp(cameraCenter, _player.PlayerEntity.Transform.Position, 0.1f));

        this._mapRenderer.Update(gameTime);
    }

    private void CheckInputs(GameTime gameTime)
    {
        if (InputManager.IsKeyDown(KeyBinding.Shoot))
        {
            var playerWeapon = _player.PlayerEntity.GetComponent<Weapon>();
            if (playerWeapon.CooldownRemaining <= 0)
            {
                Shoot(gameTime, playerWeapon, _player.PlayerEntity);
            }
        }

        if (InputManager.IsKeyDown(KeyBinding.WeaponOne))
        {
            _player.SetWeapon(WeaponTemplates.Pistol(_player.PlayerEntity));
        }
        if (InputManager.IsKeyDown(KeyBinding.WeaponTwo))
        {
            _player.SetWeapon(WeaponTemplates.AssaultRifle(_player.PlayerEntity));
        }
        if (InputManager.IsKeyDown(KeyBinding.WeaponThree))
        {
            _player.SetWeapon(WeaponTemplates.Shotgun(_player.PlayerEntity));
        }
    }

    private void Shoot(GameTime gameTime, Weapon weapon, Entity owner)
    {
        Entity[] bullets = _weaponManager.GetBullets(weapon, InputManager.GetMousePosition() + this.Camera.Position - owner.Transform.Position);

        for (int i = 0; i < bullets.Length; i++)
        {
            EntityComponentManager.AddEntity(bullets[i]);
        }

        // Update weapon cooldown
        weapon.CooldownRemaining = weapon.ShotCooldown;
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
        Console.WriteLine("Setting up new map");

        // Clear current stage
        this.EntityComponentManager.Clear();

        _tileEngine = this.EntityComponentManager.GetEngine<TileEngine>();
        this.EntityComponentManager.AddEntity(
            new Entity(_tileEngine.BuildTileGrid(_map)) { Type = EntityType.TileGrid }
        );

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

    private void ResetMap()
    {
        _player = new PlayerManager()
        {
            InputManager = this.InputManager,
            Camera = this.Camera
        };

        _player.SetSprite(ContentCache.GetClippedAsset(AssetName.Character_Brown_Idle));

        InitNewMap();
    }

    #region | Wall Colliders |
    private void CreateColliders()
    {
        var collidableLayers = _map.Layers.Where(x => x.Properties.FirstOrDefault(y => y.Key == Constants.TileMap.Properties.Collision).Value == "true");

        foreach (TiledMapTileLayer layer in collidableLayers)
        {
            ProcessLayerColliders(layer);
        }

        // Draw a rectangle around the terrain's borders
        AddCollider(new Point(-1, -1), new Point(_map.Width, -1), null);                     // Top collider
        AddCollider(new Point(-1, _map.Height), new Point(_map.Width, _map.Height), null);   // Bottom collider
        AddCollider(new Point(-1, 0), new Point(-1, _map.Height - 1), null);                 // Left left collider
        AddCollider(new Point(_map.Width, 0), new Point(_map.Width, _map.Height - 1), null); // Right collider
    }

    private void ProcessLayerColliders(TiledMapTileLayer layer)
    {
        // This tracks if a tile has a collider that accounts for it already.
        // It is used to prevent smaller colliders from generating like: [ [ [ [ [ [ [ ]
        bool[,] colliderCreated = new bool[_map.Width, _map.Height];

        for (ushort y = 0; y < layer.Height; y++)
        {
            for (ushort x = 0; x < layer.Width; x++)
            {
                ProcessTileColliders(layer, x, y, colliderCreated);
            }
        }
    }

    private void ProcessTileColliders(TiledMapTileLayer layer, ushort x, ushort y, bool[,] colliderCreated)
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
                AddCollider(firstTile.ToPoint(), lastTile.ToPoint(), colliderCreated);
            }
            else
            {
                // If a horizontal group doesn't exist, create a vertial group.
                lastTile = GetLastTile(layer, firstTile, false);

                // Don't care about GlobalIDs here, always create it. This lets 1x1 tiles generate a collider
                AddCollider(firstTile.ToPoint(), lastTile.ToPoint(), colliderCreated);
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

    private void AddCollider(Point start, Point end, bool[,] colliderCreated)
    {
        int sizeX = end.X - start.X + 1;
        int sizeY = end.Y - start.Y + 1;

        // Build out collider
        Vector2 location = new Vector2(start.X * _map.TileWidth, start.Y * _map.TileHeight);
        Point size = new Point(sizeX * _map.TileWidth, sizeY * _map.TileHeight);

        var entity = new Entity(new Component[] {
            new Transform() { Position = location },
            new BoxCollider() { LocalBoundingBox = new Rectangle(Point.Zero, size), Static = true },
        })
        {
            Name = Constants.Entities.Wall,
            Opaque = true
        };
        entity.Transform.TargetPosition = entity.Transform.Position;

        EntityComponentManager.AddEntity(entity);


        if (colliderCreated != null)
        {
            for (int x = start.X; x <= end.X; x++)
            {
                for (int y = start.Y; y <= end.Y; y++)
                {
                    colliderCreated[x, y] = true;
                }
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
                Point size = new Point(sprite.Width, sprite.Height);
                EnemyType et = Enum.Parse<EnemyType>(obj.Properties.First(x => x.Key == Constants.TileMap.Properties.EnemyType).Value);

                Entity e = new Entity()
                {
                    Name = $"Enemy-{et}",
                    Type = EntityType.Enemy
                };
                e.AddComponents(new Component[] {
                    new Transform() { Position = obj.Position },
                    new Intelligence() {
                        Implementation = et switch
                        {
                            EnemyType.None => null,
                            EnemyType.Dummy => new Intelligences.Dummy(),
                            EnemyType.Follower => new Intelligences.Follower(),
                            EnemyType.Turret => new Intelligences.Turret(),
                            _ => throw new NotImplementedException($"EnemyType of {et} is not yet implemented")
                        }
                    },
                    new Health() { MaxHealth = 50 }, // TODO: Get from map's definition
                    new Sprite() { Texture = sprite },
                    new BoxCollider() { LocalBoundingBox = new Rectangle(Point.Zero, size) },
                    new Velocity() { },
                    WeaponTemplates.Pistol(e) // TODO: Get weapon type from map's definition
                });
                e.Transform.Position -= new Vector2(0, _map.TileHeight); // TODO: Need a better solution than this

                EntityComponentManager.AddEntity(e);
            }
        }
    }
}
