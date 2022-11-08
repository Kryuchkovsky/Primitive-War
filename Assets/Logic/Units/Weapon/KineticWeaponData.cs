using System;
using UnityEngine;

namespace Logic.Units.Weapon
{
    [Serializable]
    public class WeaponData
    {
        [SerializeField] private Bullet _bullet;
        [SerializeField] [Min(1)] private int _shots;
        [SerializeField] [Min(0)] private float _damage = 100;
        [SerializeField] [Min(0)] private float _bulletSpeed = 20;
        [SerializeField] [Range(0, 1000)] private float _range;
        [SerializeField] [Range(0, 100)] private float _explosionRange;
        [SerializeField] [Range(0, 100)] private float _explosionForce;
        [SerializeField] [Min(0)] private float _reloadTimeBetweenShots = 0.25f;
        [SerializeField] [Min(0)] private float _mainReloadTime = 3;

        public Bullet Bullet => _bullet;
        public int Shots => _shots;
        public float Damage => _damage;
        public float BulletSpeed => _bulletSpeed;
        public float Range => _range;
        public float ExplosionRange => _explosionRange;
        public float ExplosionForce => _explosionForce;
        public float ReloadTimeBetweenShots => _reloadTimeBetweenShots;
        public float MainReloadTime => _mainReloadTime;
    }
}