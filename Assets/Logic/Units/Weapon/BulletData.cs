using System;
using UnityEngine;

namespace Logic.Units.Weapon
{
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