using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace TopDownShooter.ECS
{
    public abstract class Engine
    {
        /// <summary>List of entities that meet the list of RequiredComponents</summary>
        protected List<Entity> Entities;
        public abstract Type[] RequiredComponents { get; }
        public int ProcessingOrder { get; set; }
        public ReadOnlyCollection<Entity> MyEntities { get => new ReadOnlyCollection<Entity>(this.Entities); }

        public Guid ID { get; }

        protected Engine()
        {
            Entities = new List<Entity>();
            this.ID = new Guid();
        }

        public virtual void Start()
        {

        }

        public virtual void AddEntity(Entity entity)
        {
            Entities.Add(entity);
        }

        /// <param name="allEntities">All of the entities in the game</param>
        public virtual void Update(GameTime gameTime, List<Entity> allEntities)
        {
            var entitiesToRemove = new List<Entity>();

            for (int i = 0; i < this.Entities.Count(); i++) {
                var e = this.Entities[i];

                if (e.Expired) {
                    entitiesToRemove.Add(e);
                }
            }

            for (int i = 0; i < entitiesToRemove.Count(); i++) {
                var e = entitiesToRemove[i];
                this.Entities.Remove(e);
            }
        }

        public virtual void ClearEntities()
        {
            this.Entities.Clear();
        }

        public virtual void Draw(SpriteBatch sb, GraphicsDevice gd, OrthographicCamera camera)
        {

        }
    }
}
