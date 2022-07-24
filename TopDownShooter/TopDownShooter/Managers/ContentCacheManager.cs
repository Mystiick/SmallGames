using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended.Gui;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Tiled;

using MystiickCore.Models;

namespace MystiickCore.Managers;

public class ContentCacheManager
{
    private readonly ContentManager _content;
    private readonly Dictionary<string, Texture2D> _textureCache;
    private readonly Dictionary<string, TiledMap> _tiledCache;

    private TextureAtlas atlas;

    public ContentCacheManager(ContentManager content)
    {
        _content = content;

        _textureCache = new Dictionary<string, Texture2D>();
        _tiledCache = new Dictionary<string, TiledMap>();
    }

    public Texture2D LoadTileset(string asset, SpriteAtlas[] sprites)
    {
        if (!_textureCache.ContainsKey(asset))
        {
            _textureCache.Add(asset, _content.Load<Texture2D>(GetPathFromLookup(asset)));

            atlas = new TextureAtlas("base", _textureCache[asset]);
            CreateAtlasRegions(sprites);
        }

        return _textureCache[asset];
    }

    public virtual TextureRegion2D GetClippedAsset(string asset)
    {
        return atlas.GetRegion(GetPathFromLookup(asset));
    }

    public TiledMap GetTiledMap(string level)
    {
        if (!_tiledCache.ContainsKey(level))
        {
            _tiledCache.Add(level, _content.Load<TiledMap>($"Maps/{level}"));
        }

        return _tiledCache[level];
    }

    /// <summary>
    /// Get a string value for special named assets. Probably only need this for files
    /// </summary>
    private string GetPathFromLookup(string asset)
    {
        return asset switch
        {
            "Tileset" => "tilesheet_transparent", //todo: constant
            _ => asset.ToString()
        };
    }

    private void CreateAtlasRegions(SpriteAtlas[] regions)
    {
        foreach (var region in regions)
        {
            atlas.CreateRegion(region.Name, region.Position.X, region.Position.Y, region.Size.X, region.Size.Y);
        }
    }
}
