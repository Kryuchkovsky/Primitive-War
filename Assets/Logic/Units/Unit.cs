using UnityEngine;

namespace Logic.Units
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField] protected UnitType _type;
    }
}
