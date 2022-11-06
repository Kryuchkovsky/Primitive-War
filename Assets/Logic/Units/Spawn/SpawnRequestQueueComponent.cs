using System.Collections.Generic;
using Logic.Teams;

namespace Logic.Units.Spawn
{
    public struct SpawnRequestQueueComponent
    {
        public Queue<Unit> UnitPrefabs;
    }
}