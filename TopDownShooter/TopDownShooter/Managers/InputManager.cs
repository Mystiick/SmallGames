using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TopDownShooter.Interfaces;

namespace TopDownShooter.Managers
{
    public class InputManager : IInputManager
    {
        private KeyboardState keyboard;
        private MouseState mouse;

        Dictionary<KeyBinding, MouseAndKeys> bindings;
        public InputManager()
        {
            bindings = new Dictionary<KeyBinding, MouseAndKeys>
            {
                { KeyBinding.MoveUp, MouseAndKeys.W },
                { KeyBinding.MoveDown, MouseAndKeys.S },
                { KeyBinding.MoveLeft, MouseAndKeys.A },
                { KeyBinding.MoveRight, MouseAndKeys.D },
                { KeyBinding.Shoot, MouseAndKeys.LeftClick },
                { KeyBinding.Debug, MouseAndKeys.Tilde },
                { KeyBinding.ZoomIn, MouseAndKeys.R },
                { KeyBinding.ZoomOut, MouseAndKeys.F},
                { KeyBinding.WeaponOne, MouseAndKeys.One },
                { KeyBinding.WeaponTwo, MouseAndKeys.Two },
                { KeyBinding.WeaponThree, MouseAndKeys.Three },
            };
        }

        public bool IsKeyDown(KeyBinding key)
        {
            return (bindings[key]) switch
            {
                MouseAndKeys.LeftClick => mouse.LeftButton == ButtonState.Pressed,
                MouseAndKeys.RightClick => mouse.RightButton == ButtonState.Pressed,
                MouseAndKeys.ScrollClick => mouse.MiddleButton == ButtonState.Pressed,
                MouseAndKeys.ScrollUp => throw new NotImplementedException(),
                MouseAndKeys.ScrollDown => throw new NotImplementedException(),
                MouseAndKeys.Tilde => keyboard.IsKeyDown(Keys.OemTilde),
                MouseAndKeys.One => keyboard.IsKeyDown(Keys.D1),
                MouseAndKeys.Two => keyboard.IsKeyDown(Keys.D2),
                MouseAndKeys.Three => keyboard.IsKeyDown(Keys.D3),
                _ => keyboard.IsKeyDown(Enum.Parse<Keys>(bindings[key].ToString())),
            };
        }

        public Vector2 GetMousePosition()
        {
            return new Vector2(mouse.X, mouse.Y);
        }

        public void Update(GameTime gameTime)
        {
            keyboard = Keyboard.GetState();
            mouse = Mouse.GetState();
        }

        public bool DirectIsKeyDown(Keys key)
        {
            return keyboard.IsKeyDown(key);
        }
    }
}
