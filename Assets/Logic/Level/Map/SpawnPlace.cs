using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic.Level.Map
{
    [Serializable]
    public struct SpawnPlace
    {
        public List<Transform> SpawnPoints;
        [Range(0, 3)] public int Number;
    }
}