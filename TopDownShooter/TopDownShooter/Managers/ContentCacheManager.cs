using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Markup;
using MonoGame.Extended.TextureAtlases;
using TopDownShooter.UI;
using MonoGame.Extended.Tiled;

namespace TopDownShooter.Managers
{
    public class ContentCacheManager
    {
        private readonly ContentManager _content;
        private readonly MessagingManager _messagingManager;
        private readonly Dictionary<AssetName, Texture2D> _textureCache;
        private readonly Dictionary<ScreenName, IScreen> _screenCache;
        private readonly Dictionary<string, TiledMap> _tiledCache;

        private TextureAtlas atlas;

        public ContentCacheManager(ContentManager content, MessagingManager messagingManager)
        {
            _content = content;
            _messagingManager = messagingManager;

            _textureCache = new Dictionary<AssetName, Texture2D>();
            _screenCache = new Dictionary<ScreenName, IScreen>();
            _tiledCache = new Dictionary<string, TiledMap>();
        }

        public Texture2D LoadTileset(AssetName asset)
        {
            if (!_textureCache.ContainsKey(asset))
            {
                _textureCache.Add(asset, _content.Load<Texture2D>(GetPathFromLookup(asset)));

                atlas = new TextureAtlas("base", _textureCache[asset]);
                CreateAtlasRegions();
            }

            return _textureCache[asset];
        }

        public virtual TextureRegion2D GetClippedAsset(AssetName asset)
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

        public Screen GetScreen(ScreenName screen)
        {
            if (!_screenCache.ContainsKey(screen))
            {
                _screenCache.Add(screen, GetScreenByName(screen));
            }

            return _screenCache[screen].Screen;
        }

        /// <summary>
        /// Get a string value for special named assets. Probably only need this for files
        /// </summary>
        private string GetPathFromLookup(AssetName asset)
        {
            return asset switch
            {
                AssetName.Tileset => "tilesheet_transparent",
                _ => asset.ToString()
            };
        }

        private IScreen GetScreenByName(ScreenName screen)
        {
            IScreen output;

            switch (screen)
            {
                case ScreenName.None: output = new BlankScreen(); break;
                case ScreenName.MainMenu: output = new MainMenuScreen(_messagingManager); break;
                case ScreenName.Score: output = new ScoreScreen(_messagingManager); break;
                default: output = new BlankScreen(); break;
            }

            return output;
        }

        private void CreateAtlasRegions()
        {
            // TODO: Move into a file
            atlas.CreateRegion(GetPathFromLookup(AssetName.Character_Brown_Idle), 479, 2, 8, 12);
            atlas.CreateRegion(GetPathFromLookup(AssetName.Character_Orange_Pistol), 513, 36, 12, 12);
            atlas.CreateRegion(GetPathFromLookup(AssetName.Grass1), 0, 0, 16, 16);
            atlas.CreateRegion(GetPathFromLookup(AssetName.Grass2), 17, 0, 16, 16);
            atlas.CreateRegion(GetPathFromLookup(AssetName.Grass3), 34, 0, 16, 16);
            atlas.CreateRegion(GetPathFromLookup(AssetName.Grass4), 41, 0, 16, 16);
            atlas.CreateRegion(GetPathFromLookup(AssetName.Bullet), 529, 279, 3, 1);
        }
    }
}
