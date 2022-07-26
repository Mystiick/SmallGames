using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using MystiickCore;


namespace MystiickCore.Managers;

public class InputManager
{
    private KeyboardState keyboard;
    private MouseState mouse;
    private readonly Dictionary<string, MouseAndKeys> bindings;

    public InputManager(Dictionary<string, MouseAndKeys> bindings)
    {
        this.bindings = bindings;
    }

    public bool IsKeyDown(string key)
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
