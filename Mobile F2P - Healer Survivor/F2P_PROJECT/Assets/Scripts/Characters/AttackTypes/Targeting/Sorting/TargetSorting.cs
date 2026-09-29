using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared;
using System;

namespace Com.IsartDigital.HealerSurvivor.AttackTypes.Targeting.Sort
{
    public static class TargetSorting
    {
        public static int Distance(TargetStruct pA, TargetStruct pB)
        {
            return pA.distanceFromCenter.CompareTo(pB.distanceFromCenter);
        }

        public static int DistanceR(TargetStruct pA, TargetStruct pB)
        {
            return -Distance(pA, pB);
        }

        public static int Life(TargetStruct pA, TargetStruct pB)
        {
            if (pA.targetInterface == null || pB.targetInterface == null) return 0;

            return pA.targetInterface.targetHealth.CompareTo(pB.targetInterface.targetHealth);
        }

        public static int LifeR(TargetStruct pA, TargetStruct pB)
        {
            return -Life(pA, pB);
        }

        public static int MaxLife(TargetStruct pA, TargetStruct pB)
        {
            if (pA.targetInterface == null || pB.targetInterface == null) return 0;

            return pA.targetInterface.targetMaxHealth.CompareTo(pB.targetInterface.targetMaxHealth);
        }

        public static int MaxLifeR(TargetStruct pA, TargetStruct pB)
        {
            return -MaxLife(pA, pB);
        }

        public static Func<TargetStruct, TargetStruct, int> GetSortingFunc(SortTargetEnum pSortEnum)
        {
            switch (pSortEnum)
            {
                default:
                    return DistanceR;

                case SortTargetEnum.closest:
                    return DistanceR;
                case SortTargetEnum.furthest:
                    return Distance;
                case SortTargetEnum.lowestHP:
                    return LifeR;
                case SortTargetEnum.highestHP:
                    return Life;
                case SortTargetEnum.lowestMaxHP:
                    return MaxLifeR;
                case SortTargetEnum.highestMaxHP:
                    return MaxLife;
            }
        }
    }
}
