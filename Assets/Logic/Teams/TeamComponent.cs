using System;
using UnityEngine;

namespace Logic.Teams
{
    [Serializable]
    public struct TeamData
    {
        public Color Color;
        public LayerMask LayerMask;
        public int TeamIndex;
    }
}