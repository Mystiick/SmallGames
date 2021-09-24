using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

using TopDownShooter.Managers;
using TopDownShooter.Interfaces;

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

        public ContentCacheManagerWrapper() : base(null)
        {
        }

        public ContentCacheManagerWrapper(ContentManager content) : base (content)
        {
        }

        public override TextureRegion2D GetClippedAsset(AssetName asset)
        {
            return new TextureRegion2D(null, 0, 0, 10, 10);
        }
    }
}
