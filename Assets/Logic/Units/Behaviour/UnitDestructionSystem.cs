using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Logic.Units.Behaviour
{
    public class UnitDestructionSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world;

        private EcsPool<DamageComponent> _damageComponents;
        private EcsPool<UnitComponent> _unitComponents;
        private EcsPool<HealthComponent> _healthComponents;
        
        private EcsFilter _damageComponentsFilter;
        
        public void Init(IEcsSystems systems)
        {
            _damageComponents = _world.Value.GetPool<DamageComponent>();
            _unitComponents = _world.Value.GetPool<UnitComponent>();
            _healthComponents = _world.Value.GetPool<HealthComponent>();

            _damageComponentsFilter = _world.Value.Filter<DamageComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _damageComponentsFilter)
            {
                ref var damageComponent = ref _damageComponents.Get(entity);
                
                if (damageComponent.DamagedEntity.Unpack(_world.Value, out var damagedEntity))
                {
                    ref var unitComponent = ref _unitComponents.Get(damagedEntity);
                    ref var healthComponent = ref _healthComponents.Get(damagedEntity);

                    healthComponent.HealthPoints -= damageComponent.Damage;
                
                    if (healthComponent.HealthPoints <= 0)
                    {
                        if (unitComponent.Unit is SoldierUnit)
                        {
                            var solider = unitComponent.Unit as SoldierUnit;
                            solider.RagdollController.SetRagdollStatus(true);
                            solider.NavMeshAgent.enabled = false;
                        }
                        else
                        {
                            Object.Destroy(unitComponent.Unit.gameObject);
                        }

                        _world.Value.DelEntity(damagedEntity);
                    }
                }
                
                _world.Value.DelEntity(entity);
            }
        }
    }
}
