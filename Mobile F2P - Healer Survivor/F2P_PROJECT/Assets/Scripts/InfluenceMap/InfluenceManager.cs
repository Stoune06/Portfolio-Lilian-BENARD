//Clement PERSYN

using Com.IsartDigital.HealerSurvivor.InfluenceMap.Personality;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.InfluenceMap
{
    public static class InfluenceManager
    {
        private const float PROXIMITY_BOOST = (1f / InfluenceCell.DECAY_FACTOR) + (1f / InfluencePoint.UPDATE_FREQ);

        /// <summary>
        /// Return the best destination for the character on the nearest grid 
        /// </summary>
        /// <param name="pInfluenceChara"></param>
        /// <returns></returns>
        public static Vector3 GetBestDestination(InfluenceCharacter pInfluenceChara)
        {
            InfluenceCell lBestCell = GetBestCell(pInfluenceChara);
            return lBestCell.globalPosition;
        }

        public static Vector3 GetBestDestination(InfluencePoint pInfluencePoint, InfluencePersonality pPersonality)
        {
            InfluenceCell lBestCell = GetBestCell(pInfluencePoint, pPersonality);
            return lBestCell.globalPosition;
        }

        public static Vector3 GetBestDestination(Vector3 pPosition, InfluencePersonality pPersonality,
            InfluenceTypeEnum pType, InfluencePoint[] pInfluencePoints, int pRadius = 1)
        {
            InfluenceCell lBestCell = GetBestCell(pPosition,pPersonality, pType, pInfluencePoints, pRadius);
            return lBestCell.globalPosition;
        }

        public static InfluenceCell GetBestCell(InfluenceCharacter pInfluenceChara)
        {
            return GetBestCell
                (pInfluenceChara.transform.position, 
                pInfluenceChara.personality, 
                pInfluenceChara.influType, 
                pInfluenceChara.pointsToCompensate, 
                pInfluenceChara.radius);
        }

        public static InfluenceCell GetBestCell(InfluencePoint pInfluencePoint, InfluencePersonality pPersonality)
        {
            InfluenceGrid lGrid = InfluenceGrid.Get(pInfluencePoint.transform.position);

            lGrid.AddLocalInflu(pInfluencePoint.transform.position, -pInfluencePoint.myEstimateInfluence, pInfluencePoint.radius, pInfluencePoint.influType);
            lGrid.AddLocalInflu(pInfluencePoint.transform.position, PROXIMITY_BOOST, InfluenceTypeEnum.ProximityFactor);

            InfluenceCell lBestCell = lGrid.GetBestCell(pPersonality);

            lGrid.ResetLocalInflu();

            return lBestCell;
        }

        public static InfluenceCell GetBestCell(Vector3 pPosition, InfluencePersonality pPersonality, InfluenceTypeEnum pType, InfluencePoint[] pInfluencePoints, float pRadius = 1)
        {
            InfluenceGrid lGrid = InfluenceGrid.Get(pPosition);

            if ( pInfluencePoints != null)
            {
                foreach (InfluencePoint lInfluencePoint in pInfluencePoints)
                {
                    lGrid.AddLocalInflu(pPosition, -lInfluencePoint.myEstimateInfluence, pRadius, lInfluencePoint.influType);
                }
            }
            lGrid.AddLocalInflu(pPosition, PROXIMITY_BOOST, InfluenceTypeEnum.ProximityFactor);

            InfluenceCell lBestCell = lGrid.GetBestCell(pPersonality);

            lGrid.ResetLocalInflu();

            return lBestCell;
        }

        public static Vector3 GetGlobalBestDestination(Vector3 pPosition, InfluencePersonality pPersonality)
        {
            InfluenceGrid lGrid = InfluenceGrid.Get(pPosition);
            InfluenceCell lBestCell = lGrid.GetBestCell(pPersonality);

            return lBestCell.globalPosition;
        }
    }
}
