//Clement PERSYN

using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.AttackTypes.Targeting
{
    public static partial class TargetScan
    {
        private const int CIRCLE_SEGMENTS = 20;

        public static bool CheckSphere(Vector3 pCenter, float pRadius)
        {
            return CheckSphere(pCenter, pRadius, Physics.DefaultRaycastLayers);
        }

        public static bool CheckSphere(Vector3 pCenter, float pRadius, LayerMask pLayer)
        {
            int lNumColliders = Physics.OverlapSphereNonAlloc(pCenter, pRadius, hitColliderBuffer, pLayer);

            if (lNumColliders <= 0) return false;

            targetNumber = lNumColliders;

            for (int i = 0; i < lNumColliders; i++)
            {
                targetBuffer[i] = CreateTargetStruct(i, pCenter);
            }

            return true;
        }

        public static void DrawCircle(Vector3 pCenter, float pRadius)
        {
            Color lColor = Color.yellow;

            float lAngle = 0f;
            Vector3 lPosition;
            Vector3 lPreviousPosition = pCenter + new Vector3(Mathf.Cos(lAngle), 0f, Mathf.Sin(lAngle)) * pRadius;

            for (int lI = 1; lI <= CIRCLE_SEGMENTS; lI++)
            {
                lAngle = Mathf.PI * 2 * lI / CIRCLE_SEGMENTS;
                lPosition = pCenter + new Vector3(Mathf.Cos(lAngle), 0f, Mathf.Sin(lAngle)) * pRadius;

                Debug.DrawLine(lPreviousPosition, lPosition, lColor, PRE_VIEW_DURATION);
                lPreviousPosition = lPosition;
            }
        }
    }
}
