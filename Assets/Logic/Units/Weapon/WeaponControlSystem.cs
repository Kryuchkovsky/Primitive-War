using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Logic.Level.Initialization;
using Logic.Level.Map;
using UnityEngine;

namespace Logic.Units.Weapon
{
    public sealed class WeaponControlSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world;
        
        private EcsPool<MapInformationComponent> _mapInformationComponents;
        private EcsPool<KineticWeaponComponent> _kineticWeaponComponents;
        private EcsPool<WeaponReloadComponent> _weaponReloadComponents;
        private EcsPool<BulletComponent> _bulletComponents;

        private EcsFilter _mapComponentsFilter;
        private EcsFilter _filter;
        private MapHolder _map;

        public void Init(IEcsSystems systems)
        {
            _kineticWeaponComponents = _world.Value.GetPool<KineticWeaponComponent>();
            _weaponReloadComponents = _world.Value.GetPool<WeaponReloadComponent>();
            _bulletComponents = _world.Value.GetPool<BulletComponent>();
            _filter = _world.Value.Filter<KineticWeaponComponent>().Inc<WeaponReloadComponent>().End();
            
            var mapEntity = _mapComponentsFilter.GetRawEntities().First();
            _map = _mapInformationComponents.Get(mapEntity).Map;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var kineticWeaponComponent = ref _kineticWeaponComponents.Get(entity);
                ref var weaponReloadComponent = ref _weaponReloadComponents.Get(entity);

                if (kineticWeaponComponent.IsShooting && weaponReloadComponent.ShotsBeforeReload == 0)
                {
                    weaponReloadComponent.ShotsBeforeReload = kineticWeaponComponent.Data.Shots;
                }
                
                if (weaponReloadComponent.ShotsBeforeReload > 0)
                {
                    if (weaponReloadComponent.ReloadTime > 0)
                    {
                        weaponReloadComponent.ReloadTime -= Time.deltaTime;
                    }
                    else
                    {
                        MakeShot(kineticWeaponComponent, ref weaponReloadComponent);
                    }
                }
            }
        }

        private void MakeShot(KineticWeaponComponent kineticWeaponComponent, ref WeaponReloadComponent weaponReloadComponent)
        {
            var rotation = Quaternion.LookRotation(kineticWeaponComponent.ShotPoint.forward);
            var bullet = Object.Instantiate(
                kineticWeaponComponent.Data.Bullet,
                kineticWeaponComponent.ShotPoint.position,
                rotation,
                _map.BulletsContainer);
            
            weaponReloadComponent.ShotsBeforeReload -= 1;
            weaponReloadComponent.ReloadTime = weaponReloadComponent.ShotsBeforeReload > 0
                ? kineticWeaponComponent.Data.ReloadTimeBetweenShots
                : kineticWeaponComponent.Data.MainReloadTime;
            
            bullet.Rigidbody.velocity = bullet.transform.forward * kineticWeaponComponent.Data.BulletData.Speed;
            
            var bulletEntity = _world.Value.NewEntity();
            ref var bulletComponent = ref _bulletComponents.Add(bulletEntity);
            bulletComponent.Bullet = bullet;
            bulletComponent.Data = kineticWeaponComponent.Data.BulletData;
        }
    }
}