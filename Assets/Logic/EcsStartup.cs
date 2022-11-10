using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Logic.Level.Initialization;
using Logic.Level.Map;
using Logic.Mechanics.Explosions;
using Logic.Teams;
using Logic.Units;
using Logic.Units.Behaviour;
using Logic.Units.Spawn;
using Logic.Units.Weapon;
using Logic.Units.Weapon.Bullets;
using UnityEngine;

namespace Logic
{
    public sealed class EcsStartup : MonoBehaviour
    {
        [SerializeField] private MapHolder _map;
        [SerializeField] private UnitList _unitList;
        [SerializeField] private TeamsConfiguration _teamsConfiguration;
        [SerializeField] private KineticWeaponConfiguration _kineticWeaponConfiguration;

        private EcsWorld _world;
        private IEcsSystems _systems;

        private void Start()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world);
            _systems
                .Add(new MapCreationSystem())
                .Add(new SpawnRequestCreationSystem())
                .Add(new UnitSpawnSystem())
                .Add(new UnitDestructionSystem())
                .Add(new UnitMovementControlSystem())
                .Add(new EnemiesDetectionSystem())
                .Add(new WeaponControlSystem())
                .Add(new UnitBehaviourControlSystem())
                .Add(new BulletCreationSystem())
                .Add(new BulletsDestructionSystem())
                .Add(new ExplosionCreationSystem())
#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .Inject(_map)
                .Inject(_unitList)
                .Inject(_teamsConfiguration)
                .Inject(_kineticWeaponConfiguration)
                .Init();
        }

        private void Update()
        {
            _systems?.Run();
        }

        private void OnDestroy()
        {
            if (_systems != null)
            {
                _systems.Destroy();
                _systems = null;
            }
            
            if (_world != null)
            {
                _world.Destroy();
                _world = null;
            }
        }
    }
}