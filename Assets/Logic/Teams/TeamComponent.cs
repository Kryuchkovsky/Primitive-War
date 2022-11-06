using System;
using UnityEngine;

namespace Logic.Teams
{
    [Serializable]
    public struct TeamComponent
    {
        public Color Color;
        public LayerMask LayerMask;
        public int LayerIndex;
        public int TeamId;
    }
}