using Com.IsartDigital.HealerSurvivor.InfluenceMap;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes
{
    [CustomEditor(typeof(InfluenceCharacter), false)]
    public class InfluenceCharacterDrawer : InfluencePointDrawer
    {
        public override void OnInspectorGUI()
        {
            SerializedObject lPersonalitySO = null;

            serializedObject.Update();
            base.OnInspectorGUI();

            SerializedProperty lStatProperty = serializedObject.FindProperty("personality");
            EditorGUILayout.PropertyField(lStatProperty);

            if (lStatProperty.objectReferenceValue != null)
            {
                lPersonalitySO = new SerializedObject(lStatProperty.objectReferenceValue);
                lPersonalitySO.Update();
            }

            if (lStatProperty.objectReferenceValue != null)
            {

                SerializedProperty lFactorsProp = lPersonalitySO.FindProperty("_InfluenceFactorsArray");

                if (lFactorsProp != null)
                {
                    EditorGUILayout.PropertyField(lFactorsProp, true);
                }

                lPersonalitySO.ApplyModifiedProperties();
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("priorityFocusLayer"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
