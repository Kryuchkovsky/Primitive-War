using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Logic.Level.Initialization;
using Logic.Level.Map;
using UnityEngine;

namespace Logic.Units.Weapon.Bullets
{
    public sealed class BulletCreationSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world;
        
        private EcsPool<MapInformationComponent> _mapInformationComponents;
        private EcsPool<BulletSpawnRequestComponent> _bulletSpawnRequestComponents;
        private EcsPool<BulletComponent> _bulletComponents;
        private EcsPool<BulletCollisionComponent> _bulletCollisionComponents;

        private EcsFilter _mapComponentsFilter;
        private EcsFilter _bulletSpawnRequestComponentsFilter;
        private MapHolder _map;

        public void Init(IEcsSystems systems)
        {
            _mapInformationComponents = _world.Value.GetPool<MapInformationComponent>();
            _bulletSpawnRequestComponents = _world.Value.GetPool<BulletSpawnRequestComponent>();
            _bulletComponents = _world.Value.GetPool<BulletComponent>();
            _bulletCollisionComponents = _world.Value.GetPool<BulletCollisionComponent>();
            
            _mapComponentsFilter = _world.Value.Filter<MapInformationComponent>().End();
            _bulletSpawnRequestComponentsFilter = _world.Value.Filter<BulletSpawnRequestComponent>().End();
            
            var mapEntity = _mapComponentsFilter.GetRawEntities().First();
            _map = _mapInformationComponents.Get(mapEntity).Map;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _bulletSpawnRequestComponentsFilter)
            {
                ref var bulletSpawnRequestComponent = ref _bulletSpawnRequestComponents.Get(entity);

                var data = bulletSpawnRequestComponent.Data;
                var rotation = Quaternion.LookRotation(bulletSpawnRequestComponent.ShotPoint.forward);
                var bullet = Object.Instantiate(
                    bulletSpawnRequestComponent.Prefab,
                    bulletSpawnRequestComponent.ShotPoint.position,
                    rotation,
                    _map.BulletsContainer);

                bullet.Rigidbody.velocity = bullet.transform.forward * data.Speed;

                var bulletEntity = _world.Value.NewEntity();
                ref var bulletComponent = ref _bulletComponents.Add(bulletEntity);
                bulletComponent.Bullet = bullet;
                bulletComponent.Data = data;

                bullet.OnCollide += collision =>
                {
                    ref var eventEntity = ref _bulletCollisionComponents.Add(bulletEntity);
                    eventEntity.Collision = collision;
                };

                _world.Value.DelEntity(entity);
            }
        }
    }
}