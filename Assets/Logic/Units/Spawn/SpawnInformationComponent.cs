using UnityEngine;

namespace Logic.Units.Spawn
{
    public struct SpawnInformationComponent
    {
        public UnitData SpawningUnitData;
        [Min(0)] public float TimeBeforeSpawn;
        public bool UnitIsSpawning;
    }
}