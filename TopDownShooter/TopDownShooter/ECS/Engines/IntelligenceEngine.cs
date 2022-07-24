using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

using MystiickCore;
using MystiickCore.ECS;
using MystiickCore.ECS.Components;
using MystiickCore.Services;

using TopDownShooter.Intelligences;

namespace TopDownShooter.ECS.Engines
{
    public class IntelligenceEngine : Engine
    {
        public override Type[] RequiredComponents => new Type[] { typeof(Transform), typeof(Intelligence) };

        private TileGrid _grid;

        public override void Start()
        {
            base.Start();

            MessagingService.Subscribe(EventType.GameEvent, MystiickCore.Constants.GameEvent.EnemyKilled, (s, a) => { UpdateEnemyCount(); }, this.ID);
            MessagingService.Subscribe(EventType.GameEvent, MystiickCore.Constants.GameEvent.MapGridReset, UpdateIntelligences, this.ID);
        }

        public override void Update(GameTime gameTime, List<Entity> allEntities)
        {
            base.Update(gameTime, allEntities);

            _grid = ParentEntityComponentManager.WorldTileGrid?.GetComponent<TileGrid>();

            for (int i = 0; i < this.Entities.Count; i++)
            {
                var x = this.Entities[i];

                var intel = x.GetComponent<Intelligence>();
                if (intel.Implementation != null)
                {
                    intel.Implementation.PlayerEntity = ParentEntityComponentManager.PlayerEntity;
                    intel.Implementation.CurrentEntity = x;
                    ((ShooterIntelligence)intel.Implementation).Grid = _grid;

                    intel.Implementation.Update(gameTime, allEntities);
                }
            }
        }

        public override void AddEntity(Entity entity)
        {
            base.AddEntity(entity);

            var intel = entity.GetComponent<Intelligence>();


            switch (entity.Type)
            {
                case EntityType.Enemy:
                    UpdateEnemyCount();
                    break;
            }
        }

        private void UpdateEnemyCount()
        {
            MessagingService.SendMessage(EventType.Score, Constants.Score.EnemyCountUpdated, this, this.Entities.Count(x => x.Type == EntityType.Enemy && !x.Expired));
        }

        private void UpdateIntelligences(object sender, object args)
        {
            // Something has changed, and we need 
            foreach (Entity e in this.Entities)
            {
                e.GetComponent<Intelligence>().Implementation.PlayerInformationChanged();
            }
        }
    }
}