using UnityEngine;

namespace Logic.Units.Spawn
{
    public struct SpawnInformationComponent
    {
        public int TeamId;
        [Min(0)] public float TimeBeforeSpawn;
    }
}