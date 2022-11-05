using UnityEngine;

namespace Logic.Units.Spawn
{
    public struct SpawnInformationComponent
    {
        public UnitData SpawningUnitData;
        public int TeamId;
        [Min(0)] public float TimeBeforeSpawn;
        public bool UnitIsSpawning;
    }
}