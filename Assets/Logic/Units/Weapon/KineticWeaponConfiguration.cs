using UnityEngine;
using UnityEngine.Rendering;

namespace Logic.Units.Weapon
{
    [CreateAssetMenu(menuName = "Create KineticWeaponConfiguration", fileName = "KineticWeaponConfiguration", order = 0)]
    public class KineticWeaponConfiguration : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<KineticWeaponType, KineticWeaponData> _weaponData;

        public KineticWeaponData GetDataByType(KineticWeaponType type) => _weaponData[type];
    }
}
