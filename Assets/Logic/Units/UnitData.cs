using System;
using UnityEngine;

namespace Logic.Units
{
    [Serializable]
    public class UnitData
    {
        [SerializeField] private Unit _prefab;
        [SerializeField] [Min(0)] private float _attackRange = 30;
        [SerializeField] [Min(0)] private float _spawningTime = 3;
        [SerializeField] [Min(1)] private float _initialHealthPoints = 100;
        [SerializeField] private UnitType _type;

        public Unit Prefab => _prefab;
        public float AttackRange => _attackRange;
        public float SpawningTime => _spawningTime;
        public float InitialHealthPoints => _initialHealthPoints;
        public UnitType Type => _type;
    }
}