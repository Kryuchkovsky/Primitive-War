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
                ref var unitComponent = ref _unitComponents.Get(damageComponent.Entity);
                ref var healthComponent = ref _healthComponents.Get(damageComponent.Entity);

                healthComponent.HealthPoints -= damageComponent.Damage;
                
                if (healthComponent.HealthPoints <= 0)
                {
                    Object.Destroy(unitComponent.Unit.gameObject);
                    _world.Value.DelEntity(damageComponent.Entity);
                }
                
                _world.Value.DelEntity(entity);
            }
        }
    }
}
