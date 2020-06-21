using System    ;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TopDownShooter.ECS.Components;
using TopDownShooter.Intelligences;

namespace TopDownShooter.ECS.Engines
{
    public class IntelligenceEngine : Engine
    {
        public override Type[] RequiredComponents => new Type[] { typeof(Transform), typeof(Health), typeof(Intelligence) };

        private Dictionary<EnemyType, IImplementation> _implementations;
        private Entity _playerEntity, _gridEntity;
        private TileGrid _grid;

        public override void Start()
        {
            base.Start();

            // Build out instance pool for Intelligence implementations
            _implementations = new Dictionary<EnemyType, IImplementation>();
            _implementations.Add(EnemyType.None, null);
            _implementations.Add(EnemyType.Dummy, new Dummy());
            _implementations.Add(EnemyType.Turret, new Turret());
            _implementations.Add(EnemyType.Follower, new Follower());
        }

        public override void Update(GameTime gameTime, List<Entity> allEntities)
        {
            base.Update(gameTime, allEntities);

            // TODO: There just has to be a better way
            if (_playerEntity == null || _playerEntity.Expired)
            {
                Entity _playerEntity = allEntities.FirstOrDefault(x => x.Name == Constants.Entities.Player);
            }
            if (_gridEntity == null || _playerEntity.Expired)
            {
                Entity _gridEntity = allEntities.FirstOrDefault(x => x.HasComponent<TileGrid>());
            }
            if (_gridEntity != null && _grid == null)
            {
                _grid = _gridEntity.GetComponent<TileGrid>();
            }

            for (int i = 0; i < this.Entities.Count; i++)
            {
                var x = this.Entities[i];

                var intel = x.GetComponent<Intelligence>();
                if (intel.Implementation != null)
                {
                    intel.Implementation.PlayerEntity = _playerEntity;
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

            if (entity.Name == Constants.Entities.Player)
            {
                _playerEntity = entity;
            }

            if (entity.HasComponent<TileGrid>())
            {
                _gridEntity = entity;
            }
        }
    }
}