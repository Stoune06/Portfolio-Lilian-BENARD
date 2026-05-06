//Clement PERSYN

using Com.IsartDigital.HealerSurvivor.InfluenceMap.Personality;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

namespace Com.IsartDigital.HealerSurvivor.InfluenceMap
{
    public class InfluenceCharacter : InfluencePoint
    {
        public InfluencePersonality personality;
        public LayerMask priorityFocusLayer;

        public Vector3 targetVelocity;
        public LayerMask targetLayerMask;

        [HideInInspector] public InfluencePoint[] pointsToCompensate;

        private void Awake()
        {
            pointsToCompensate = GetComponentsInChildren<InfluencePoint>(false);
        }
    }
}
