using MystiickCore.ECS;
using MystiickCore.Managers;

namespace MystiickCore.Tests;

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
