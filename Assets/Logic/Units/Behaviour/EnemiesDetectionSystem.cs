using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Logic.Teams;
using Logic.Units.Weapon;
using UnityEngine;

namespace Logic.Units.Behaviour
{
    public sealed class EnemiesDetectionSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;

        private Collider[] _colliders = new Collider[1];
        
        private EcsPool<UnitComponent> _unitComponents;
        private EcsPool<ShootingComponent> _shootingComponents;
        private EcsPool<KineticWeaponComponent> _kineticWeaponComponents;
        private EcsPool<TeamComponent> _teamComponents;

        private EcsFilter _filter;

        public void Init(IEcsSystems systems)
        {
            _unitComponents = _world.Value.GetPool<UnitComponent>();
            _shootingComponents = _world.Value.GetPool<ShootingComponent>();
            _kineticWeaponComponents = _world.Value.GetPool<KineticWeaponComponent>();
            _teamComponents = _world.Value.GetPool<TeamComponent>();
            
            _filter = _world.Value
                .Filter<UnitComponent>()
                .Inc<ShootingComponent>()
                .Inc<KineticWeaponComponent>()
                .Inc<TeamComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var unitComponent = ref _unitComponents.Get(entity);
                ref var shootingComponent = ref _shootingComponents.Get(entity);
                ref var kineticWeaponComponent = ref _kineticWeaponComponents.Get(entity);
                ref var teamComponent = ref _teamComponents.Get(entity);

                if (shootingComponent.Target)
                {
                    var distance = (shootingComponent.Target.transform.position - unitComponent.Unit.transform.position).magnitude;
                    var canShooting = distance > kineticWeaponComponent.Data.BulletData.Distance;
                    shootingComponent.Target = canShooting ? shootingComponent.Target : null;
                }
                else
                {
                    var matches = Physics.OverlapSphereNonAlloc(
                        unitComponent.Unit.transform.position, 
                        kineticWeaponComponent.Data.BulletData.Distance, 
                        _colliders, 
                        teamComponent.EnemiesLayerMask);

                    for (int i = 0; i < matches; i++)
                    {
                        if (shootingComponent.Target) break;

                        if (_colliders[i].TryGetComponent(out Unit unit))
                        {
                            shootingComponent.Target = unit;
                        }
                    }
                }
            }
        }
    }
}