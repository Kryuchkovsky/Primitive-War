using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic.Level.Map
{
    public class MapHolder : MonoBehaviour
    {
        [SerializeField] private List<SpawnPlace> _spawnPlaces;
        [SerializeField] private Transform _captureZone;
        
        public List<SpawnPlace> SpawnPlaces => _spawnPlaces;
        public Vector3 CaptureZonePosition => _captureZone.position;
    }

    [Serializable]
    public struct SpawnPlace
    {
        public List<Transform> SpawnPoints;
        [Range(0, 3)] public int Number;
    }
}
