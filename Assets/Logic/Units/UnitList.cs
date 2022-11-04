using UnityEngine;
using UnityEngine.Rendering;

namespace Logic.Units
{
    [CreateAssetMenu(menuName = "Create UnitList", fileName = "UnitList", order = 0)]
    public class UnitList : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<UnitType, Unit> _units;

        public Unit GetUnitByType(UnitType type) => _units[type];
    }
}