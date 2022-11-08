using UnityEngine;

namespace Logic.Units.Weapon
{
    public class WeaponHolder : MonoBehaviour
    {
        [SerializeField] private Transform _shotPoint;
        [SerializeField] private Transform _pointForLeftHand;
        [SerializeField] private Transform _pointForRightHand;

        public Transform ShotPoint => _shotPoint;
        public Transform PointForLeftHand => _pointForLeftHand;
        public Transform PointForRightHand => _pointForRightHand;
    }
}