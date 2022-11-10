using UnityEngine;

namespace Logic.Units.Weapon
{
    public class ManualWeaponHolder : MonoBehaviour
    {
        [SerializeField] private Transform _shotPoint;
        
        public Transform ShotPoint => _shotPoint;
    }
}