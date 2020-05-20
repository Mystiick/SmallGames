using System;
using System.Collections.Generic;
using System.Text;
using TopDownShooter.ECS;
using TopDownShooter.ECS.Components;
using TopDownShooter.ECS.Engines;
using TopDownShooter.Managers;

namespace TopDownShooter.Tests
{
    public static class TestHelper
    {
        public static void AddEngines(EntityComponentManager unit, params Engine[] engines)
        {
            for (int i = 0; i < engines.Length; i++)
            {
                unit.AddEngine(engines[i], i);
            }
        }

    }
}
