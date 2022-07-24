using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MystiickCore.ECS.Components;

public class Sprite : Component
{
    private TextureRegion2D _texture;

    public TextureRegion2D Texture
    {
        get
        {
            return _texture;
        }
        set
        {
            _texture = value;
            this.Origin = value.Size / 2;
        }
    }
    public Vector2 Origin;

}
