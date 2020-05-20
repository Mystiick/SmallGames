using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TopDownShooter
{
    public static class Helpers
    {
        public static float DetermineAngle(Vector2 v1, Vector2 v2)
        {
            return (float)Math.Atan2(v2.Y - v1.Y, v2.X - v1.X);
        }
    }
}
