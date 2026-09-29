//Clement PERSYN

using Com.IsartDigital.HealerSurvivor.AttackTypes.Targeting.Sort;
using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.AttackTypes.Targeting
{
    public static partial class TargetScan
    {
        private const int MAX_TARGET = 128;
        private const float PRE_VIEW_DURATION = 1f;

        public static Collider[] hitColliderBuffer = new Collider[MAX_TARGET];
        public static TargetStruct[] targetBuffer = new TargetStruct[MAX_TARGET];
        public static RaycastHit[] rayHitBuffer = new RaycastHit[MAX_TARGET];
        public static int targetNumber;

        public static TargetStruct CreateTargetStruct(int pIndex, Vector3 pCenter)
        {
            Transform lTransform = hitColliderBuffer[pIndex].transform;
            float lSqrDistance = (lTransform.position - pCenter).sqrMagnitude;

            return new TargetStruct(lTransform, lSqrDistance);
        }

        public static TargetStruct GetBest(Func<TargetStruct, TargetStruct, int> pSortingFunc)
        {
            if (targetNumber <= 0)
            {
                return default;
            }

            TargetStruct lTargetStruct;
            TargetStruct lBestTarget = targetBuffer[0];

            for (int i = 1; i < targetNumber; i++)
            {
                lTargetStruct = targetBuffer[i];
                if (pSortingFunc(lTargetStruct, lBestTarget) == 1) lBestTarget = lTargetStruct;
            }

            return lBestTarget;
        }

        public static bool CheckInRange(Vector3 pCenter, float pRadius, LayerMask pLayer)
        {
            return Physics.CheckSphere(pCenter, pRadius, pLayer);
        }


        public static TargetStruct[] GetArray()
        {
            return targetBuffer;
        }

        public static TargetStruct[] GetArray(out int pTargetNumber)
        {
            pTargetNumber = targetNumber;
            return targetBuffer;
        }

        public static TargetStruct[] GetSortedArray(Func<TargetStruct, TargetStruct, int> pSortingFunc)
        {
            Array.Sort(targetBuffer, 0, targetNumber, Comparer<TargetStruct>.Create((pA, pB) => pSortingFunc(pA, pB)));

            return targetBuffer;
        }

        public static TargetStruct[] GetSortedArray(Func<TargetStruct, TargetStruct, int> pSortingFunc, out int pTargetNumber)
        {
            pTargetNumber = targetNumber;
            return GetSortedArray(pSortingFunc);
        }

    }
}
