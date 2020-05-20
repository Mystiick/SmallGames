using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownShooter.ECS.Components
{
    public class Health : Component
    {
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }

        public Health()
        {
        }

        public Health(int max)
        {
            this.CurrentHealth = max;
            this.MaxHealth = max;
        }
    }
}
