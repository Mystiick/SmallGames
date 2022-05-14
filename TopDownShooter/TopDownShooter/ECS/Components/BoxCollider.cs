using System;
using Microsoft.Xna.Framework;

namespace TopDownShooter.ECS.Components
{
    public class BoxCollider : Component
    {
        public string[] Tags { get; set; }
        public Rectangle LocalBoundingBox { get; set; }
        /// <summary>Describes the bounding box's place in this world</summary>
        public Rectangle WorldBoundingBox { get; set; }
        public Rectangle TargetBoundingBox { get; set; }
        public Action<Entity, Entity> OnCollisionEnter { get; set; } // Action, or Events or whatever here
        public Action<Entity, Entity> OnCollisionHitMe { get; set; } // Action, or Events or whatever here
        public bool Trigger { get; set; }
        /// <summary>Determines if we need continous collision detection, or if current position is acceptable</summary>
        public bool Continuous { get; set; }
        public bool Static { get; set; }

        public BoxCollider Copy()
        {
            return new BoxCollider()
            {
                Tags = this.Tags,
                LocalBoundingBox = this.LocalBoundingBox,
                WorldBoundingBox = this.WorldBoundingBox,
                TargetBoundingBox = this.TargetBoundingBox,
                Trigger = this.Trigger,
                Continuous = this.Continuous,
                Static = this.Static
            };
        }
    }
}
