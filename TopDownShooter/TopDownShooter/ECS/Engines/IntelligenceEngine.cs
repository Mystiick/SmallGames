using System;
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

        public override void Start()
        {
            base.Start();

            // Build out instance pool for Intelligence implementations
            _implementations = new Dictionary<EnemyType, IImplementation>();
            _implementations.Add(EnemyType.None, null);
            _implementations.Add(EnemyType.Dummy, new Dummy());
            _implementations.Add(EnemyType.Turret, new Turret());
        }

        public override void Update(GameTime gameTime, List<Entity> allEntities)
        {
            base.Update(gameTime, allEntities);

            Entity playerEntity = allEntities.FirstOrDefault(x => x.Name == Constants.Entities.Player);

            for (int i = 0; i < this.Entities.Count; i++)
            {
                var x = this.Entities[i];

                var intel = x.GetComponent<Intelligence>();
                if (intel.Implementation != null)
                {
                    intel.Implementation.PlayerEntity = playerEntity;
                    intel.Implementation.CurrentEntity = x;

                    intel.Implementation.Update(gameTime);
                }
                
            }
        }

        public override void AddEntity(Entity entity)
        {
            base.AddEntity(entity);

            var intel = entity.GetComponent<Intelligence>();
            intel.Implementation = _implementations[intel.EnemyType];
        }
    }
}