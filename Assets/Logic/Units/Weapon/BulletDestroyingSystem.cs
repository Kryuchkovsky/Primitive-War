using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Logic.Units.Weapon
{
    public sealed class BulletDestroyingSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world;
        
        private EcsPool<BulletComponent> _bulletComponents;

        private EcsFilter _filter;

        public void Init(IEcsSystems systems)
        {
            _bulletComponents = _world.Value.GetPool<BulletComponent>();
            _filter = _world.Value.Filter<BulletComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var bulletComponent = ref _bulletComponents.Get(entity);

                if (bulletComponent.TraveledDistance >= bulletComponent.Data.Distance)
                {
                    Object.Destroy(bulletComponent.Bullet.gameObject);
                    _world.Value.DelEntity(entity);
                }
                else
                {
                    var delta = bulletComponent.Bullet.Rigidbody.velocity.magnitude * Time.deltaTime;
                    bulletComponent.TraveledDistance += delta;
                }
            }
        }
    }
}