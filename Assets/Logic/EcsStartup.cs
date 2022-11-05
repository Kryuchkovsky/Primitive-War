using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Logic.Level.Initialization;
using Logic.Level.Map;
using Logic.Units;
using Logic.Units.Spawn;
using UnityEngine;

namespace Logic
{
    public sealed class EcsStartup : MonoBehaviour
    {
        [SerializeField] private MapHolder _map;
        [SerializeField] private UnitList _unitList;

        private EcsWorld _world;
        private IEcsSystems _systems;

        private void Start()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world);
            _systems
                .Add(new MapCreationSystem())
                .Add(new SpawnRequestCreationSystem())
                .Add(new SpawnUnitSystem())
                // register your systems here, for example:
                // .Add (new TestSystem1 ())
                // .Add (new TestSystem2 ())

                // register additional worlds here, for example:
                // .AddWorld (new EcsWorld (), "events")
#if UNITY_EDITOR
                // add debug systems for custom worlds here, for example:
                // .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ("events"))
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .Inject(_map)
                .Inject(_unitList)
                .Init();
        }

        private void Update()
        {
            // process systems here.
            _systems?.Run();
        }

        private void OnDestroy()
        {
            if (_systems != null)
            {
                // list of custom worlds will be cleared
                // during IEcsSystems.Destroy(). so, you
                // need to save it here if you need.
                _systems.Destroy();
                _systems = null;
            }

            // cleanup custom worlds here.

            // cleanup default world.
            if (_world != null)
            {
                _world.Destroy();
                _world = null;
            }
        }
    }
}