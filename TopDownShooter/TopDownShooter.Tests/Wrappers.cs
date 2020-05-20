using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TopDownShooter.ECS.Managers;
using TopDownShooter.ECS.Components;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using TopDownShooter.Managers;
using Moq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using TopDownShooter.Interfaces;
using Microsoft.Xna.Framework.Content;

namespace TopDownShooter.Tests
{
    public class InputManagerWrapper : IInputManager
    {
        public Func<KeyBinding, bool> MockIsKeyDown;
        public Vector2 MockGetMousePosition;
        public Action<GameTime> MockUpdate;

        public bool IsKeyDown(KeyBinding key)
        {
            if (MockIsKeyDown != null)
            {
                return MockIsKeyDown(key);
            }
            else
            {
                return key switch
                {
                    KeyBinding.MoveUp => true,
                    KeyBinding.MoveLeft => true,
                    _ => false
                };
            }
        }

        public Vector2 GetMousePosition()
        {
            return MockGetMousePosition;
        }

        public void Update(GameTime gameTime)
        {
            MockUpdate?.Invoke(gameTime);
        }
    }

    public class ViewportAdapterWrapper : ViewportAdapter
    {
        public ViewportAdapterWrapper(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        public override int VirtualWidth => 0;

        public override int VirtualHeight => 0;

        public override int ViewportWidth => 0;

        public override int ViewportHeight => 0;

        public override Matrix GetScaleMatrix()
        {
            return new Matrix();
        }
    }

    public class RandomWrapper : Random
    {
        public override int Next(int minValue, int maxValue)
        {
            return maxValue;
        }
    }

    public class ContentCacheManagerWrapper : ContentCacheManager
    {

        public ContentCacheManagerWrapper() : base(null, null)
        {
        }

        public ContentCacheManagerWrapper(ContentManager content, MessagingManager messagingManager) : base (content, messagingManager)
        {
        }

        public override TextureRegion2D GetClippedAsset(AssetName asset)
        {
            return new TextureRegion2D(null, 0, 0, 10, 10);
        }
    }
}
