//Clement PERSYN
using Com.IsartDigital.HealerSurvivor.InfluenceMap.Personality;
using System;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.InfluenceMap
{
    public class InfluenceCell
    {
        public const float DECAY_FACTOR = 0.25f;
        public const float MIN_INFLU= 0.1f;

        public Vector2Int gridPosition;
        public Vector3 globalPosition;
        public Vector2 size;

        private float[] _InfluByTypes;
        private float[] _LocalInfluByTypes;

        private static readonly int _Length = System.Enum.GetValues(typeof(InfluenceTypeEnum)).Length;

        public InfluenceCell(Vector2Int pGridPosition, Vector2 pSize, Vector2 pOriginGrid, float pHeight)
        {
            _InfluByTypes = new float[_Length];
            _LocalInfluByTypes = new float[_Length];

            gridPosition = pGridPosition;
            globalPosition = new Vector3
                        (
                        pOriginGrid.x + pSize.x * pGridPosition.x + pSize.x * 0.5f,
                        pHeight + 0.1f,
                        pOriginGrid.y + pSize.y * pGridPosition.y + pSize.y * 0.5f
                        );
            size = pSize;
        }

        public static void AddInflu(InfluenceCell pCell, InfluenceTypeEnum pType, float pValue)
        {
            pCell._InfluByTypes[(int)pType] += pValue;
        }

        public static void AddLocalInflu(InfluenceCell pCell, InfluenceTypeEnum pType, float pValue)
        {
            pCell._LocalInfluByTypes[(int)pType] += pValue;
        }

        public void ResetLocalInflu()
        {
            Array.Clear(_LocalInfluByTypes, 0, _LocalInfluByTypes.Length);
        }

        public void Update(float pDecayMultiplier)
        {
            for (int i = _Length - 1; i >= 0 ; i--)
            {
                if (_InfluByTypes[i] == 0) continue;

                if (Mathf.Abs(_InfluByTypes[i]) < MIN_INFLU)
                {
                    _InfluByTypes[i] = 0;
                    continue;
                }

                _InfluByTypes[i] *= pDecayMultiplier;
            }
        }

        public float GetInfluence(InfluencePersonality pPersonality, bool pAddLocalInflu = false)
        {
            float lInfluence = 0;
            float lFactor;

            for (int i = _Length - 1; i >= 0; i--)
            {
                lFactor = pPersonality != null ? pPersonality.influenceFactors[(InfluenceTypeEnum)i] : 1f;
                    lInfluence += _InfluByTypes[i] * lFactor;
                if (pAddLocalInflu) lInfluence += _LocalInfluByTypes[i] * lFactor;
            }

            return lInfluence;
        }
    }
}