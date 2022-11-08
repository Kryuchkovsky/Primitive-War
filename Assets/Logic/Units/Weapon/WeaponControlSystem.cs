using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Logic.Units.Weapon
{
    public sealed class WeaponControlSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world;
        
        private EcsPool<KineticWeaponComponent> _kineticWeaponComponents;
        private EcsPool<WeaponReloadComponent> _weaponReloadComponents;
        
        private EcsFilter _filter;

        public void Init(IEcsSystems systems)
        {
            _kineticWeaponComponents = _world.Value.GetPool<KineticWeaponComponent>();
            _weaponReloadComponents = _world.Value.GetPool<WeaponReloadComponent>();
            _filter = _world.Value.Filter<KineticWeaponComponent>().Inc<WeaponReloadComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref var kineticWeaponComponent = ref _kineticWeaponComponents.Get(entity);
                ref var weaponReloadComponent = ref _weaponReloadComponents.Get(entity);

                if (kineticWeaponComponent.IsShooting || weaponReloadComponent.ShotsBeforeReload > 0)
                {
                    if (weaponReloadComponent.ReloadTime > 0)
                    {
                        weaponReloadComponent.ReloadTime -= Time.deltaTime;
                    }
                    else
                    {
                        var rotation = Quaternion.LookRotation(kineticWeaponComponent.ShotPoint.forward);
                        var bullet = Object.Instantiate(
                            kineticWeaponComponent.Data.Bullet,
                            kineticWeaponComponent.ShotPoint.position,
                            rotation);
                        weaponReloadComponent.ShotsBeforeReload -= 1;
                        weaponReloadComponent.ReloadTime = weaponReloadComponent.ShotsBeforeReload > 0
                            ? kineticWeaponComponent.Data.ReloadTimeBetweenShots
                            : kineticWeaponComponent.Data.MainReloadTime;
                    }
                }
            }
        }
    }
}