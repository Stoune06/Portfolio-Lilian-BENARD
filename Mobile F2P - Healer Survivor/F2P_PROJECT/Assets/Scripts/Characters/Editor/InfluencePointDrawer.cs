using Com.IsartDigital.HealerSurvivor.InfluenceMap;
using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes
{
    [CustomEditor(typeof(InfluencePoint), false)]
    public class InfluencePointDrawer : Editor
    {
        private const float MARGIN = 10f;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("influType"), GUIContent.none);

            Rect lControlRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight * 0.8f);

            Rect lLeftRect = new Rect(lControlRect.x, lControlRect.y, lControlRect.width / 2f - MARGIN, lControlRect.height);
            Rect lRightRect = new Rect(lControlRect.x + lControlRect.width / 2f + MARGIN, lControlRect.y, lControlRect.width / 2f - MARGIN, lControlRect.height);

            EditorGUI.LabelField(lLeftRect, "Facteur d'Influence");
            EditorGUI.LabelField(lRightRect, "Rayon");

            lControlRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            lLeftRect = new Rect(lControlRect.x, lControlRect.y, lControlRect.width / 2f - MARGIN, lControlRect.height);
            lRightRect = new Rect(lControlRect.x + lControlRect.width / 2f + MARGIN, lControlRect.y, lControlRect.width / 2f - MARGIN, lControlRect.height);

            EditorGUI.PropertyField(lLeftRect, serializedObject.FindProperty("influence"), GUIContent.none);
            EditorGUI.PropertyField(lRightRect, serializedObject.FindProperty("radius"), GUIContent.none);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
