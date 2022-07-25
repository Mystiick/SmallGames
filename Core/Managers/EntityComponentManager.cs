using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;

using MystiickCore.ECS;
using MystiickCore.Exceptions;
using MystiickCore.Services;

namespace MystiickCore.Managers;

public abstract class EntityComponentManager
{

    private readonly List<Entity> _entities;
    private readonly Guid _guid;
    protected IOrderedEnumerable<Engine> _engines;

    public int EngineCount { get { return _engines.Count(); } }
    public ReadOnlyCollection<Entity> MyEntities { get => new ReadOnlyCollection<Entity>(_entities); }
    
    // Common entities
    public Entity PlayerEntity { get; private set; }
    public Entity WorldTileGrid { get; private set; }

    public EntityComponentManager()
    {
        _entities = new List<Entity>();
        _engines = new List<Engine>().OrderBy(x => 0);
        _guid = new Guid();
    }

    public virtual void Init()
    {
        MessagingService.Subscribe(EventType.Spawn, OnMessageReceived, _guid);
    }

    public void AddEngine(Engine item, int processingOrder)
    {
        if (_engines.Any(x => x.ProcessingOrder == processingOrder))
        {
            throw new DuplicateEngineProcessingOrderException(processingOrder);
        }

        item.ParentEntityComponentManager = this;
        item.ProcessingOrder = processingOrder;
        item.Start();
        _engines = _engines.Concat(new Engine[] {item}).OrderBy(x => x.ProcessingOrder);
    }

    public T GetEngine<T>() where T : Engine
    {
        T engine = (T)_engines.FirstOrDefault((x) => x is T);

        return engine;
    }

    public void AddEntity(Entity item)
    {
        _entities.Add(item);

        // Find engines that require the components this entity has
        foreach (Engine e in _engines.Where(x => x.RequiredComponents.All(y => item.HasComponent(y))))
        {
            e.AddEntity(item);
        }

        switch (item.Type)
        {
            case EntityType.Player:
                Console.WriteLine($"Updating Player Entity: {item.ID}");
                this.PlayerEntity = item;
                break;
            case EntityType.TileGrid:
                this.WorldTileGrid = item;
                break;
        }
    }

    public void Update(GameTime gameTime)
    {
        foreach (Engine e in _engines)
        {
            e.Update(gameTime, _entities);
        }

        foreach (Entity e in _entities.Where(x => x.Expired).ToArray())
        {
            // Remove it from the list of all entities
            _entities.Remove(e);
        }
    }

    public void Draw(SpriteBatch sb, GraphicsDevice gd, OrthographicCamera camera)
    {
        foreach (Entity e in _entities)
        {
            if (e.Sprite?.Texture != null && e.Transform != null)
            {
                sb.Draw(e.Sprite.Texture, e.Transform.Position + e.Sprite.Origin, Color.White, e.Transform.Rotation, e.Sprite.Origin, Vector2.One, SpriteEffects.None, 0f);
            }
        }

        foreach (Engine e in _engines)
        {
            e.Draw(sb, gd, camera);
        }
    }

    /// <summary>
    /// Removes all entities from the entity list
    /// </summary>
    public void Clear()
    {
        // Expire all entities, some places hold onto their own references they don't need to lookup forever
        // If the entity doesn't get expired, then it will hold onto the old refernce, even though _entities is being cleared
        foreach (Entity ent in _entities)
        {
            ent.Expired = true;
        }
        _entities.Clear();

        foreach (Engine eng in _engines)
        {
            eng.ClearEntities();
        }
    }

    private void OnMessageReceived(object sender, object args) 
    {
        if (args is Entity)
        {
            this.AddEntity(args as Entity);
        }
    }
}
