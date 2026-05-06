//Clement PERSYN

using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared;
using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.AttackTypes.Targeting
{
    public static partial class TargetScan
    {
        private const int SEGMENTS = 20;

        public static bool CheckCone(Vector3 pCenter, Vector3 pDirection, float pMaxAngle, float pRadius)
        {
            if (pMaxAngle >= 360f) return CheckSphere(pCenter, pRadius, Physics.DefaultRaycastLayers);
            return CheckCone(pCenter,pDirection, pMaxAngle, pRadius, Physics.DefaultRaycastLayers);
        }

        public static bool CheckCone(Vector3 pCenter, Vector3 pDirection, float pMaxAngle, float pRadius, LayerMask pLayer)
        {
            if (pMaxAngle >= 360f) return CheckSphere(pCenter, pRadius, pLayer);

            int lNumColliders = Physics.OverlapSphereNonAlloc(pCenter, pRadius, hitColliderBuffer, pLayer);

            if (lNumColliders <= 0) return false;

            int lIndex = 0;

            float lCosThreshold = Mathf.Cos(pMaxAngle * 0.5f * Mathf.Deg2Rad);
            float lDistanceSqr;
            float lDot;

            Vector3 lNormalizedConeDir = pDirection.normalized;
            Vector3 lTargetDir;

            for (int i = 0; i < lNumColliders; i++)
            {
                lTargetDir = hitColliderBuffer[i].transform.position - pCenter;
                lDistanceSqr = lTargetDir.sqrMagnitude;

                if (lDistanceSqr <= 0f) continue;

                lDot = Vector3.Dot(lTargetDir.normalized, lNormalizedConeDir);

                if (lDot < lCosThreshold) continue;

                targetBuffer[lIndex] = new TargetStruct(hitColliderBuffer[i].transform, lDistanceSqr);
                lIndex++;
            }

            targetNumber = lIndex;
            return targetNumber > 0;
        }

        public static void DrawCone(Vector3 pCenter, Vector3 pDirection, float pMaxAngle, float pRadius)
        {
            Color lColor = Color.yellow;

            float lHalfAngle = pMaxAngle * 0.5f;
            Vector3 lLeftRayDirection = Quaternion.Euler(0, -lHalfAngle, 0) * pDirection;
            Vector3 lRightRayDirection = Quaternion.Euler(0, lHalfAngle, 0) * pDirection;

            Debug.DrawLine(pCenter, pCenter + lLeftRayDirection * pRadius, lColor, PRE_VIEW_DURATION);
            Debug.DrawLine(pCenter, pCenter + lRightRayDirection * pRadius, lColor, PRE_VIEW_DURATION);

            Vector3 lPreviousPoint = pCenter + lLeftRayDirection * pRadius;
            float lAngleStep = pMaxAngle / SEGMENTS;

            float lCurrentAngle;
            Vector3 lCurrentDir;
            Vector3 lNextPoint;

            for (int lI = 1; lI <= SEGMENTS; lI++)
            {
                lCurrentAngle = -lHalfAngle + (lI * lAngleStep);
                lCurrentDir = Quaternion.Euler(0, lCurrentAngle, 0) * pDirection;
                lNextPoint = pCenter + lCurrentDir * pRadius;

                Debug.DrawLine(lPreviousPoint, lNextPoint, lColor, PRE_VIEW_DURATION);
                lPreviousPoint = lNextPoint;
            }
        }
    }
}
