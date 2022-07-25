using Microsoft.Xna.Framework;

using MystiickCore.Managers;

namespace MystiickCore.Tests;

internal class TestEntityComponentManager : EntityComponentManager
{
}

internal class TestInputManager : Interfaces.IInputManager
{
    public Vector2 GetMousePosition()
    {
        return Vector2.Zero;
    }

    public bool IsKeyDown(string key)
    {
        return true;
    }

    public void Update(GameTime gameTime)
    {

    }
}