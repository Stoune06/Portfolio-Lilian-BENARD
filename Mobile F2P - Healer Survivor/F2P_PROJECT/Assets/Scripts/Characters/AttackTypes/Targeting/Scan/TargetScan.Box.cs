//Clement PERSYN

using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.AttackTypes.Targeting
{
    public static partial class TargetScan
    {
        public static bool CheckBox(Vector3 pStart, Vector3 pDirection, float pRange, float pWidth)
        {
            return CheckBox(pStart, pDirection, pRange, pWidth, Physics.DefaultRaycastLayers);
        }

        public static bool CheckBox(Vector3 pStart, Vector3 pDirection, float pRange, float pWidth, LayerMask pLayer)
        {
            Quaternion lOrientation = Quaternion.LookRotation(pDirection);

            Vector3 lCenter = pStart + pDirection.normalized * (pRange * 0.5f);
            Vector3 lHalfExtents = new Vector3(pWidth * 0.5f, 1f, pRange * 0.5f);

            int lNumColliders = Physics.OverlapBoxNonAlloc(lCenter, lHalfExtents, hitColliderBuffer, lOrientation, pLayer);

            if (lNumColliders <= 0) return false;

            targetNumber = lNumColliders;

            for (int i = 0; i < lNumColliders; i++)
            {
                targetBuffer[i] = CreateTargetStruct(i, lCenter);
            }

            return true;
        }

        public static void DrawBox(Vector3 pStart, Vector3 pDirection, float pSizeY, float pSizeX)
        {
            Quaternion lRotation = Quaternion.LookRotation(pDirection);

            float lHalfX = pSizeX * 0.5f;

            Vector3 lLocalBottomLeft = new Vector3(-lHalfX, 0, 0);
            Vector3 lLocalBottomRight = new Vector3(lHalfX, 0, 0);
            Vector3 lLocalTopRight = new Vector3(lHalfX, 0, pSizeY);
            Vector3 lLocalTopLeft = new Vector3(-lHalfX, 0, pSizeY);

            Vector3 lBottomLeft = pStart + (lRotation * lLocalBottomLeft);
            Vector3 lBottomRight = pStart + (lRotation * lLocalBottomRight);
            Vector3 lTopRight = pStart + (lRotation * lLocalTopRight);
            Vector3 lTopLeft = pStart + (lRotation * lLocalTopLeft);

            Color lColor = Color.yellow;

            Debug.DrawLine(lBottomLeft, lBottomRight, lColor, PRE_VIEW_DURATION);
            Debug.DrawLine(lBottomRight, lTopRight, lColor, PRE_VIEW_DURATION);
            Debug.DrawLine(lTopRight, lTopLeft, lColor, PRE_VIEW_DURATION);
            Debug.DrawLine(lTopLeft, lBottomLeft, lColor, PRE_VIEW_DURATION);
        }
    }
}
