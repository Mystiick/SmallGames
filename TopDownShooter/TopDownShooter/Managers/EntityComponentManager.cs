using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using TopDownShooter.ECS;
using TopDownShooter.ECS.Components;
using TopDownShooter.ECS.Engines;
using TopDownShooter.Exceptions;

namespace TopDownShooter.Managers
{
    public class EntityComponentManager
    {

        private readonly List<Entity> _entities;
        private IOrderedEnumerable<Engine> _engines;

        public int EngineCount { get { return _engines.Count(); } }
        public ReadOnlyCollection<Entity> MyEntities { get => new ReadOnlyCollection<Entity>(_entities); }

        public EntityComponentManager()
        {
            _entities = new List<Entity>();
            _engines = new List<Engine>().OrderBy(x => 0);
        }

        public void Init()
        {
            // Engines are processed in this order
            int i = 0;
            AddEngine(new TimeToLiveEngine(), i++);
            AddEngine(new HealthEngine(), i++);
            AddEngine(new TileEngine(), i++);
            AddEngine(new TransformEngine(), i++);
            AddEngine(new PhysicsEngine(), i++);
            AddEngine(new IntelligenceEngine(), i++);
        }

        public void AddEngine(Engine item, int processingOrder)
        {
            if (_engines.Any(x => x.ProcessingOrder == processingOrder))
            {
                throw new DuplicateEngineProcessingOrderException(processingOrder);
            }

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
                var sprite = e.GetComponent<Sprite>();
                var transform = e.GetComponent<Transform>();

                if (sprite?.Texture != null && transform != null)
                {
                    sb.Draw(sprite.Texture, transform.Position, Color.White, transform.Rotation, sprite.Origin, Vector2.One, SpriteEffects.None, 0f);
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
            _entities.Clear();
        }
    }
}
