using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TopDownShooter.ECS.Components;
using TopDownShooter.Intelligences;
using TopDownShooter.Services;

namespace TopDownShooter.ECS.Engines
{
    public class IntelligenceEngine : Engine
    {
        public override Type[] RequiredComponents => new Type[] { typeof(Transform), typeof(Health), typeof(Intelligence) };

        private Dictionary<EnemyType, BaseIntelligence> _implementations;
        private TileGrid _grid;

        public override void Start()
        {
            base.Start();

            // Build out instance pool for Intelligence implementations
            _implementations = new Dictionary<EnemyType, BaseIntelligence>();
            _implementations.Add(EnemyType.None, null);
            _implementations.Add(EnemyType.Dummy, new Dummy());
            _implementations.Add(EnemyType.Turret, new Turret());
            _implementations.Add(EnemyType.Follower, new Follower());

            MessagingService.Subscribe(EventType.GameEvent, (s,a) => { UpdateEnemyCount(); }, this.ID);
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
                    intel.Implementation.Grid = _grid;

                    intel.Implementation.Update(gameTime, allEntities);
                }
                
            }
        }

        public override void AddEntity(Entity entity)
        {
            base.AddEntity(entity);

            var intel = entity.GetComponent<Intelligence>();
            intel.Implementation = _implementations[intel.EnemyType];

            switch (entity.Type)
            {
                case EntityType.Enemy:
                    UpdateEnemyCount();
                    break;
            }
        }

        public void UpdateEnemyCount()
        {
            MessagingService.SendMessage(EventType.Score, Constants.Score.EnemyCountUpdated, this, this.Entities.Count(x => x.Type == EntityType.Enemy && !x.Expired));
        }
    }
}