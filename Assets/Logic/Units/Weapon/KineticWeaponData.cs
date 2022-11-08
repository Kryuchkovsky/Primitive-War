using System;
using UnityEngine;

namespace Logic.Units.Weapon
{
    [Serializable]
    public class KineticWeaponData
    {
        [SerializeField] private Bullet _bullet;
        [SerializeField] private BulletData _bulletData;
        [SerializeField] [Min(1)] private int _shots = 1;
        [SerializeField] [Min(0)] private float _reloadTimeBetweenShots = 0.25f;
        [SerializeField] [Min(0)] private float _mainReloadTime = 3;

        public Bullet Bullet => _bullet;
        public BulletData BulletData => _bulletData;
        public int Shots => _shots;
        public float ReloadTimeBetweenShots => _reloadTimeBetweenShots;
        public float MainReloadTime => _mainReloadTime;
    }

    [Serializable]
    public struct BulletData
    {
        [Min(0)] public float Damage;
        [Min(0)] public float Speed;
        [Min(0)] public float Distance;
        [Min(0)] public float ExplosionRange;
        [Min(0)] public float ExplosionForce;
    }
}