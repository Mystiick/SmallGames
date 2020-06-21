using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using TopDownShooter.ECS;
using TopDownShooter.ECS.Components;
using TopDownShooter.Interfaces;
using TopDownShooter.Managers;

namespace TopDownShooter.ECS.Managers
{
    public class PlayerManager
    {
        public IInputManager InputManager { get; set; }
        public OrthographicCamera Camera { get; set; }

        public Entity PlayerEntity
        {
            get;
        }

        public PlayerManager()
        {
            PlayerEntity = new Entity(
                new Transform(),
                new BoxCollider() { BoundingBox = new Rectangle(0, 0, 10, 10) },
                new Sprite(),
                new Velocity(),
                new Health() { MaxHealth = 3, CurrentHealth = 3 }
            ) {
                Name = Constants.Entities.Player,
            };
        }

        public void SetSprite(TextureRegion2D texture)
        {
            var sprite = PlayerEntity.GetComponent<Sprite>();
            sprite.Texture = texture;
        }

        public void SetWeapon(Weapon w)
        {
            if (PlayerEntity.HasComponent<Weapon>()) 
            {
                PlayerEntity.RemoveComponent<Weapon>();
            }

            PlayerEntity.AddComponent(w);
        }

        public void Update(GameTime gameTime)
        {
            UpdateMovementAndRotation(gameTime);

            var playerWeapon = PlayerEntity.GetComponent<Weapon>();
            if (playerWeapon != null)
            {
                playerWeapon.CooldownRemaining -= gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        private void UpdateMovementAndRotation(GameTime gameTime)
        {
            var velocity = PlayerEntity.GetComponent<Velocity>();

            velocity.Speed = 150f;
            velocity.Direction = Vector2.Zero;

            if (InputManager.IsKeyDown(KeyBinding.MoveUp))
            {
                velocity.Direction += new Vector2(0, -1);
            }

            if (InputManager.IsKeyDown(KeyBinding.MoveDown))
            {
                velocity.Direction += new Vector2(0, 1);
            }

            if (InputManager.IsKeyDown(KeyBinding.MoveLeft))
            {
                velocity.Direction += new Vector2(-1, 0);
            }

            if (InputManager.IsKeyDown(KeyBinding.MoveRight))
            {
                velocity.Direction += new Vector2(1, 0);
            }

            if (InputManager.IsKeyDown(KeyBinding.ZoomIn))
            {
                this.Camera.Zoom *= 1 + (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (InputManager.IsKeyDown(KeyBinding.ZoomOut))
            {
                this.Camera.Zoom /= 1 + (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            PlayerEntity.Transform.Rotation = Helpers.DetermineAngle(PlayerEntity.Transform.Position, InputManager.GetMousePosition() + this.Camera.Position);
        }

    }
}
