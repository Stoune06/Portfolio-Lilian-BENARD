//Clement PERSYN

using Com.IsartDigital.HealerSurvivor.InfluenceMap.Personality;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.InfluenceMap
{
    public class InfluenceGrid : MonoBehaviour
    {
        [SerializeField] private int _CuttingPrecision;

        public InfluenceCell[,] cells;

        private Vector2 _CellSizes;

        private static List<InfluenceGrid> _Maps = new List<InfluenceGrid>();

        public static InfluenceGrid Get(Vector3 pPosition)
        {
            if (_Maps == null || _Maps.Count == 0) return null;

            InfluenceGrid lClosestMap = _Maps[0];
            float lMinSqrDistance = (lClosestMap.transform.position - pPosition).sqrMagnitude;

            Vector3 lOffset;
            float lSqrDistance;
            for (int i = 1; i < _Maps.Count; i++)
            {
                lOffset = _Maps[i].transform.position - pPosition;
                lSqrDistance = lOffset.sqrMagnitude;

                if (lOffset.sqrMagnitude > lMinSqrDistance) continue;

                lMinSqrDistance = lSqrDistance;
                lClosestMap = _Maps[i];
            }

            return lClosestMap;
        }

        public void CreateCells()
        {
            if (_CuttingPrecision <= 0) return;

            Vector2 lMapSize = new Vector2(transform.localScale.x, transform.localScale.z);
            Vector2 lMapPos = new Vector2(transform.position.x, transform.position.z);

            Vector2 lOrigineGrid = lMapPos - lMapSize * 0.5f;
            float lHeight = transform.position.y + transform.localScale.y * 0.5f;

            _CellSizes = lMapSize / _CuttingPrecision; 

            cells = new InfluenceCell[_CuttingPrecision, _CuttingPrecision];

            for (int lX = 0; lX < _CuttingPrecision; lX++)
            {
                for (int lY = 0; lY < _CuttingPrecision; lY++) 
                {
                    cells[lX,lY] = new InfluenceCell(new Vector2Int(lX, lY), _CellSizes, lOrigineGrid, lHeight);
                }
            }
        }

        public bool IsInsideGrid(Vector2Int pCoord)
        {
            return pCoord.x >= 0 && pCoord.x < _CuttingPrecision &&
                   pCoord.y >= 0 && pCoord.y < _CuttingPrecision;
        }

        public Vector2Int WorldToGrid(Vector3 pWorldPos)
        {
            return new Vector2Int(
                Mathf.FloorToInt((pWorldPos.x - (transform.position.x - transform.localScale.x * 0.5f)) / _CellSizes.x),
                Mathf.FloorToInt((pWorldPos.z - (transform.position.z - transform.localScale.z * 0.5f)) / _CellSizes.y)
            );
        }

        public void AddGlobalInfluence(Vector3 pCenter, float pStrength, float pRadiusWorld, InfluenceTypeEnum pType)
        {
            int lRadiusGrid = Mathf.RoundToInt(pRadiusWorld / _CellSizes.x);
            AddInfluence(WorldToGrid(pCenter), pStrength, lRadiusGrid, pType, InfluenceCell.AddInflu);
        }

        public void AddInfluence(Vector2Int pCenter, float pStrength, int pRadius, InfluenceTypeEnum pType, Action<InfluenceCell, InfluenceTypeEnum, float> pAddInfluenceMethode)
        {
            InfluenceCell lCell;

            InfluenceUtils.BufferInfluenceBlur(pCenter, pStrength, pRadius);
            foreach (InfluenceStruct lInfluStruct in InfluenceUtils.influenceBuffer)
            {
                if (!IsInsideGrid(lInfluStruct.gridPos)) continue;
                lCell = cells[lInfluStruct.gridPos.x, lInfluStruct.gridPos.y];
                pAddInfluenceMethode(lCell, pType, lInfluStruct.influenceValue);
            }
        }

        public void AddLocalInflu(Vector3 pCenter, float pStrength, float pRadiusWorld, InfluenceTypeEnum pType)
        {
            int lRadiusGrid = Mathf.RoundToInt(pRadiusWorld / _CellSizes.x);
            AddInfluence(WorldToGrid(pCenter), pStrength, lRadiusGrid, pType, InfluenceCell.AddLocalInflu);
        }

        public void AddLocalInflu(Vector3 pCenter, float pStrength, InfluenceTypeEnum pType)
        {
            AddLocalInflu(WorldToGrid(pCenter), pStrength, pType);
        }

        public void AddLocalInflu(Vector2Int pCenter, float pStrength, InfluenceTypeEnum pType)
        {
            AddInfluence(pCenter, pStrength, _CuttingPrecision, pType, InfluenceCell.AddLocalInflu);
        }

        public InfluenceCell GetBestCell(InfluencePersonality pPersonality)
        {
            if (cells == null) return null;

            InfluenceCell lBestCell = null;
            float lBestValue = -Mathf.Infinity;

            float lValue;

            foreach (InfluenceCell lCell in cells)
            {
                lValue = lCell.GetInfluence(pPersonality, true);

                if (lValue < lBestValue) continue;

                lBestCell = lCell;
                lBestValue = lValue;
            } 

            return lBestCell;
        }

        public void ResetLocalInflu()
        {
            if (cells == null) return;

            foreach (InfluenceCell lCell in cells)
            {
                lCell.ResetLocalInflu();
            }
        }

        public void Update()
        {
            float lDecayMultiplier = Mathf.Pow(InfluenceCell.DECAY_FACTOR, Time.deltaTime);
            foreach (InfluenceCell lCell in cells)
            {
                lCell.Update(lDecayMultiplier);
            }
        }

        void Awake()
        {
            CreateCells();
        }

        private void OnEnable()
        {
            _Maps.Add(this);
        }

        private void OnDisable()
        {
            _Maps.Remove(this);
        }
    }
}
