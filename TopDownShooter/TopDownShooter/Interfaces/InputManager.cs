using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TopDownShooter.Interfaces
{
    public interface IInputManager
    {
        bool IsKeyDown(KeyBinding key);
        public Vector2 GetMousePosition();
        public void Update(GameTime gameTime);
    }
}
