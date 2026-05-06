using Com.IsartDigital.HealerSurvivor.InfluenceMap;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes
{
    [CustomEditor(typeof(AttackType), false)]
    public class AttackTypeDrawer : Editor
    {
        private const float MARGIN = 10f;

        private bool _IsEditing;
        private Editor _StatEditor;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty lStatProperty = serializedObject.FindProperty("_AttackStats");
            EditorGUILayout.PropertyField(lStatProperty);

            if (lStatProperty.objectReferenceValue != null)
            {
                _IsEditing = EditorGUILayout.Foldout(_IsEditing, "Edition", true);
                if (_IsEditing)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    if (_StatEditor == null || _StatEditor.target != lStatProperty.objectReferenceValue)
                    {
                        if (_StatEditor != null) DestroyImmediate(_StatEditor);
                        _StatEditor = CreateEditor(lStatProperty.objectReferenceValue);
                    }

                    
                    _StatEditor.OnInspectorGUI();
                    
                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel--;
                }
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_AttackLayer"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackPriority"));
        }
    }
}