using System.Linq;
using System.Collections.Generic;

using MystiickCore.ECS;
using MystiickCore.ECS.Engines;

using TopDownShooter.ECS.Engines;

namespace TopDownShooter.Managers;

public class ShooterEntityComponentManager : MystiickCore.Managers.EntityComponentManager
{
    public ShooterEntityComponentManager() : base() { }

    public override void Init()
    {
        _engines = new List<Engine>().OrderBy(x => 0);

        // Engines are processed in this order
        int i = 0;
        AddEngine(new TimeToLiveEngine(), i++);
        AddEngine(new HealthEngine(), i++);
        AddEngine(new TileEngine(), i++);
        AddEngine(new TransformEngine(), i++);
        AddEngine(new PhysicsEngine(), i++);
        AddEngine(new IntelligenceEngine(), i++);

        base.Init();
    }
}
