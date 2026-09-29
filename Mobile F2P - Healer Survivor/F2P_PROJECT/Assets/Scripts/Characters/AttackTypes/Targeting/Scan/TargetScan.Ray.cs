//Clement PERSYN

using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared;
using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.AttackTypes.Targeting
{
    public static partial class TargetScan
    {
        public static bool CheckRay(Vector3 pCenter, Vector3 pDirection, float pRange)
        {
            return CheckRay(pCenter, pDirection, pRange, Physics.DefaultRaycastLayers);
        }

        public static bool CheckRay(Vector3 pCenter, Vector3 pDirection, float pRange, LayerMask pLayer)
        {
            int lNumColliders = Physics.RaycastNonAlloc(pCenter, pDirection, rayHitBuffer, pRange, pLayer);

            if (lNumColliders <= 0) return false;

            targetNumber = lNumColliders;

            for (int i = 0; i < lNumColliders; i++)
            {
                targetBuffer[i] = CreateTargetStruct(i, pCenter, rayHitBuffer);
            }

            return true;
        }


        public static TargetStruct CreateTargetStruct(int pIndex, Vector3 pCenter, RaycastHit[] rayHitBuffer)
        {
            Transform lTransform = rayHitBuffer[pIndex].transform;
            float lSqrDistance = (lTransform.position - pCenter).sqrMagnitude;

            return new TargetStruct(lTransform, lSqrDistance);
        }

        public static void DrawRay(Vector3 pCenter, Vector3 pDirection, float pRange)
        {
            Debug.DrawRay(pCenter, pDirection * pRange,Color.yellow, PRE_VIEW_DURATION);
        }
    }
}