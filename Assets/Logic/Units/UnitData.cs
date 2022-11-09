using System;
using UnityEngine;

namespace Logic.Units
{
    [Serializable]
    public class UnitData
    {
        [SerializeField] private Unit _prefab;
        [SerializeField] [Min(0)] private int _spawningTime = 3;
        [SerializeField] [Min(1)] private float _initialHealthPoints = 100;

        public Unit Prefab => _prefab;
        public int SpawningTime => _spawningTime;
        public float InitialHealthPoints => _initialHealthPoints;
    }
}