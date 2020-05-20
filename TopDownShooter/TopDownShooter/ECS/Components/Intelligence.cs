using System;
using System.Collections.Generic;
using System.Text;
using TopDownShooter.Intelligences;

namespace TopDownShooter.ECS.Components
{
    public class Intelligence : Component
    {
        public EnemyType EnemyType { get; set; }
        public IImplementation Implementation { get; set; }
    }
}
