using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Logic.Units.Render
{
    [CreateAssetMenu(menuName = "Create UnitRenderDataList", fileName = "UnitRenderDataList", order = 0)]
    public class UnitRenderDataList : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<UnitType, UnitRenderData> _unitRenderData;

        public UnitType[] GetTypes() => _unitRenderData.Keys.ToArray();
        public UnitRenderData GetDataByType(UnitType type) => _unitRenderData[type];
    }
}