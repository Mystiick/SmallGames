using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

using MystiickCore;
using MystiickCore.Managers;

namespace TopDownShooter.Tests;

internal class InputManagerWrapper : InputManager
{
    public Func<string, bool> MockIsKeyDown;
    public Vector2 MockGetMousePosition;
    public Action<GameTime> MockUpdate;

    public InputManagerWrapper() : base(new Dictionary<string, MouseAndKeys>())
    {
    }

    public bool IsKeyDown(string key)
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

    public Vector2 GetMousePosition() => MockGetMousePosition;

    public void Update(GameTime gameTime)
    {
        MockUpdate?.Invoke(gameTime);
    }

    public bool DirectIsKeyDown(Keys key) => true;
}

internal class ViewportAdapterWrapper : ViewportAdapter
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

internal class RandomWrapper : Random
{
    public override int Next(int minValue, int maxValue)
    {
        return maxValue;
    }
}

internal class ContentCacheManagerWrapper : ContentCacheManager
{

    public ContentCacheManagerWrapper() : base(null)
    {
    }

    public ContentCacheManagerWrapper(ContentManager content) : base (content)
    {
    }

    public override TextureRegion2D GetClippedAsset(string asset)
    {
        return new TextureRegion2D(null, 0, 0, 10, 10);
    }
}
