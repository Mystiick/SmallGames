using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MystiickCore;
using MystiickCore.ECS;
using MystiickCore.ECS.Components;
using MystiickCore.Managers;
using MystiickCore.Models;
using MystiickCore.Services;

using TopDownShooter.Managers;

namespace TopDownShooter.Stages
{
    public class DebugStage : BaseStage
    {

        private PlayerManager _player;
        private MonoGame.Extended.Tiled.Renderers.TiledMapRenderer _mapRenderer;
        private MonoGame.Extended.Tiled.TiledMap _map;

        public DebugStage() : base()
        {
            _player = new PlayerManager();
        }

        public override void LoadContent(ContentCacheManager contentManager)
        {
            base.LoadContent(contentManager);
            ContentCache.LoadTileset(AssetName.Tileset, Constants.SpriteAtlas);

            _player.SetSprite(ContentCache.GetClippedAsset(AssetName.Character_Brown_Idle));
        }

        public override void Start()
        {
            base.Start();

            MessagingService.SendMessage(EventType.UserInterface, Constants.UserInterface.SetActive, this, ScreenName.Score);
            MessagingService.SendMessage(EventType.Score, Constants.Score.PlayerScoreUpdated, this, 0);

            EntityComponentManager.AddEntity(_player.PlayerEntity);
            _player.InputManager = this.InputManager;
            _player.Camera = this.Camera;

            LoadMap();

            var playerSpawn = this._map.ObjectLayers.First(x => x.Name == "Triggers").Objects.First(x => x.Name == "PlayerSpawn");

            _player.PlayerEntity.Transform.Position = playerSpawn.Position;


            var enemy = new Entity(new Component[] {
                new Sprite() { Texture = ContentCache.GetClippedAsset(AssetName.Character_Brown_Idle) },
                new Transform() { Position = new Vector2(518, 354) },
                new BoxCollider() { LocalBoundingBox = new Rectangle(-1, 2, 10, 10) },
                new Velocity()
            });
            enemy.Name = "Enemy";
            enemy.GetComponent<Sprite>().Origin = new Vector2(enemy.GetComponent<Sprite>().Texture.Width, enemy.GetComponent<Sprite>().Texture.Height) / 2;
            EntityComponentManager.AddEntity(enemy);


            var enemy2 = new Entity(new Component[] {
                new Sprite() { Texture = ContentCache.GetClippedAsset(AssetName.Character_Brown_Idle) },
                new Transform() { Position = new Vector2(558, 454) },
                new BoxCollider() { LocalBoundingBox = new Rectangle(-1, 2, 10, 10), Trigger = true },
                new Velocity()
            });
            enemy2.Name = "Enemy2";
            enemy2.GetComponent<Sprite>().Origin = new Vector2(enemy2.GetComponent<Sprite>().Texture.Width, enemy2.GetComponent<Sprite>().Texture.Height) / 2;
            EntityComponentManager.AddEntity(enemy2);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _player.Update(gameTime);

            // Lerp camera to player
            Vector2 cameraCenter = this.Camera.Position + this.Camera.Origin;
            this.Camera.LookAt(Vector2.Lerp(cameraCenter, _player.PlayerEntity.Transform.Position, 0.1f));

            MessagingService.SendMessage(EventType.Score, Constants.Score.PlayerScoreUpdated, this, this.Camera.Zoom);

            this._mapRenderer.Update(gameTime);
        }

        public override void Draw()
        {
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            this._mapRenderer.Draw(Camera.GetViewMatrix());

            base.Draw();
        }

        private void LoadMap()
        {
            _map = ContentCache.GetTiledMap("MiniComplex");
            _mapRenderer = new MonoGame.Extended.Tiled.Renderers.TiledMapRenderer(GraphicsDevice);
            _mapRenderer.LoadMap(_map);
        }
    }
}
