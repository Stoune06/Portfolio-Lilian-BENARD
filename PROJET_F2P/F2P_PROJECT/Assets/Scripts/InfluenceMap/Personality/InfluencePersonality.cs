//Clement PERSYN

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.InfluenceMap.Personality
{
    [CreateAssetMenu(fileName = "Personality", menuName = "Scriptable Objects/Personality")]
    public class InfluencePersonality : ScriptableObject
    {
        [SerializeField] private InfluenceTypeStruct[] _InfluenceFactorsArray;
        public Dictionary<InfluenceTypeEnum, float> influenceFactors = new Dictionary<InfluenceTypeEnum, float>();

        private void OnEnable()
        {
            SetFactors();
        }

        private void SetFactors()
        {
            foreach (InfluenceTypeStruct lInfluenceStruct in _InfluenceFactorsArray)
            {
                influenceFactors[lInfluenceStruct.influenceTypeEnum] = lInfluenceStruct.value;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            int lLastLength = 0;
            InfluenceTypeStruct[] lLastInfluence = null;
            if (_InfluenceFactorsArray != null)
            {
                lLastInfluence = (InfluenceTypeStruct[])_InfluenceFactorsArray.Clone();
                lLastLength = lLastInfluence.Length;
            }

            string[] lArray = System.Enum.GetNames(typeof(InfluenceTypeEnum));
            int lLength = lArray.Length;

            _InfluenceFactorsArray = new InfluenceTypeStruct[lLength];

            for (int i = 0; i < lLength; i++)
            {
                _InfluenceFactorsArray[i].name = lArray[i];
                _InfluenceFactorsArray[i].influenceTypeEnum = (InfluenceTypeEnum)i;
                if (lLastInfluence != null && i < lLastLength) _InfluenceFactorsArray[i].value = lLastInfluence[i].value;
            }
        }
#endif
    }

    [Serializable]
    public struct InfluenceTypeStruct
    {
        [HideInInspector] public string name;
        [HideInInspector] public InfluenceTypeEnum influenceTypeEnum;

        public float value;
    }

    public enum InfluenceTypeEnum
    {
        Enemy,
        Allie,
        AllieRange,
        Player,
        Interest,
        ProximityFactor,
    }
}
