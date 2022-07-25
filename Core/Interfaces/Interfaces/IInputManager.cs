using Microsoft.Xna.Framework;

namespace MystiickCore.Interfaces;

public interface IInputManager
{
    bool IsKeyDown(string key);
    public Vector2 GetMousePosition();
    public void Update(GameTime gameTime);
}
