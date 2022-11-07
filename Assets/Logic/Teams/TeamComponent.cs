using System;
using UnityEngine;

namespace Logic.Teams
{
    [Serializable]
    public struct TeamComponent
    {
        public Color Color;
        public LayerMask EnemiesLayerMask;
        public int LayerIndex;
        public int TeamId;
    }
}