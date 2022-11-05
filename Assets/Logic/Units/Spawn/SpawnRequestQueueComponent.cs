using System.Collections.Generic;

namespace Logic.Units.Spawn
{
    public struct SpawnRequestQueueComponent
    {
        public Queue<Unit> UnitPrefabs;
        public int TeamId;
    }
}