using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using MystiickCore.Managers;

using System.Collections.Generic;

namespace MystiickCore.Tests;

internal class TestEntityComponentManager : EntityComponentManager
{
}

internal class TestInputManager : InputManager
{
    public TestInputManager() : base(new Dictionary<string, MouseAndKeys>()) { }

    public Vector2 GetMousePosition() => Vector2.Zero;
    public bool IsKeyDown(string key) => true;
    public void Update(GameTime gameTime) { }
    public bool DirectIsKeyDown(Keys key) => true;
}