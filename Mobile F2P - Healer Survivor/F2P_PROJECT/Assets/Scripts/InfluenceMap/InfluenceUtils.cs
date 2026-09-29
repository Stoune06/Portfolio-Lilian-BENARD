//Clement PERSYN

using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.InfluenceMap
{
    public static class InfluenceUtils
    {
        public static List<InfluenceStruct> influenceBuffer = new List<InfluenceStruct>(256);

        public static List<InfluenceStruct> BufferInfluenceBlur(Vector2Int pCenter, float pStrength, int pRadius)
        {
            Vector2Int lCurrentCoord;
            InfluenceStruct lInfluenceStruct;

            float lDist;
            float lInfluence;
            int lSide = (pRadius * 2) + 1;

            influenceBuffer.Clear();

            for (int lX = -pRadius; lX <= pRadius; lX++)
            {
                for (int lY = -pRadius; lY <= pRadius; lY++)
                {
                    lCurrentCoord = new Vector2Int(pCenter.x + lX, pCenter.y + lY);

                    lDist = Vector2Int.Distance(pCenter, lCurrentCoord);
                    lInfluence = Mathf.Abs(pStrength) * (1f - (lDist / pRadius));
                    lInfluence = Mathf.Max(0, lInfluence);

                    lInfluenceStruct = new InfluenceStruct
                    {
                        gridPos = lCurrentCoord,
                        influenceValue = lInfluence * Mathf.Sign(pStrength),
                    };

                    influenceBuffer.Add(lInfluenceStruct);
                }
            }

            return influenceBuffer;
        }
    }

    public struct InfluenceStruct
    {
        public Vector2Int gridPos;
        public float influenceValue;
    }
}


