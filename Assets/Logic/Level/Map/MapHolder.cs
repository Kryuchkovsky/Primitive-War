using System.Collections.Generic;
using UnityEngine;

namespace Logic.Level.Map
{
    public class MapHolder : MonoBehaviour
    {
        [SerializeField] private List<SpawnPlace> _spawnPlaces;
        [SerializeField] private Transform _unitsContainer;
        [SerializeField] private Transform _bulletsContainer;
        [SerializeField] private Transform _captureZone;
        
        public List<SpawnPlace> SpawnPlaces => _spawnPlaces;
        public Transform UnitsContainer => _unitsContainer;
        public Transform BulletsContainer => _bulletsContainer;
        public Vector3 CaptureZonePosition => _captureZone.position;
    }
}
