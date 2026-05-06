using Com.IsartDigital.HealerSurvivor.InfluenceMap.Personality;
using System;
using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.InfluenceMap
{
    [CustomEditor(typeof(InfluencePersonality))]
    public class PersonalityDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("NORMALIZE")) NormalizePersonality();
        }

        private void NormalizePersonality()
        {
            serializedObject.Update();
            SerializedProperty lArray = serializedObject.FindProperty("_InfluenceFactorsArray");
            int lSize = lArray.arraySize;
            float lAbsTotal = 0f;

            for (int i = 0; i < lSize; i++)
            {
                lAbsTotal += Mathf.Abs(lArray.GetArrayElementAtIndex(i).FindPropertyRelative("value").floatValue);
            }

            if (lAbsTotal > 0)
            {
                for (int i = 0; i < lSize; i++)
                {
                    SerializedProperty lProp = lArray.GetArrayElementAtIndex(i).FindPropertyRelative("value");
                    lProp.floatValue /= lAbsTotal;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
