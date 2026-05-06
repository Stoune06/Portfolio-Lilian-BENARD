using Tooling;
using UnityEditor;
using UnityEngine;

//Author : MERFOUD Kelyan

namespace Editors.Translation
{
    [CustomPropertyDrawer(typeof(Translable<>), true)]
    public class TranslableDrawer : PropertyDrawer
    {
        private const float MARGIN = 16f;
        private const float HEIGHT = 2f;
        private const string VALUE_ARRAY = "ValueArray";

        public override void OnGUI(Rect pPosition, SerializedProperty pProperty, GUIContent pContent)
        {
            EditorGUI.BeginProperty(pPosition, pContent, pProperty);

            SerializedProperty lValueArray = pProperty.FindPropertyRelative(VALUE_ARRAY);
            if (lValueArray == null)
            {
                EditorGUI.LabelField(pPosition, "Not Found");
                EditorGUI.EndProperty();
                return;
            }
            lValueArray.arraySize = Language.Languages.Length;

            Rect lFoldoutRect = new Rect(pPosition.x, pPosition.y, pPosition.width, EditorGUIUtility.singleLineHeight);
            pProperty.isExpanded = EditorGUI.Foldout(lFoldoutRect, pProperty.isExpanded, pContent, true);

            if (pProperty.isExpanded)
            {
                float pHeight = pPosition.y + EditorGUIUtility.singleLineHeight + HEIGHT;
                Rect lFieldRect = new Rect(pPosition.x + MARGIN + EditorGUI.indentLevel, pHeight, pPosition.width, EditorGUIUtility.singleLineHeight);
                string lLanguage;
                SerializedProperty lProperty;

                for (int i = 0; i < Language.Languages.Length; i++)
                {
                    lLanguage = Language.Languages[i];
                    lProperty = lValueArray.GetArrayElementAtIndex(i);

                    lFieldRect.y = pHeight;
                    EditorGUI.PropertyField(lFieldRect, lProperty, new GUIContent(lLanguage));

                    pHeight += EditorGUIUtility.singleLineHeight + HEIGHT;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty pProperty, GUIContent pLabel)
        {
            float pHeight = EditorGUIUtility.singleLineHeight;

            if (pProperty.isExpanded)
                pHeight += EditorGUIUtility.singleLineHeight * Language.Languages.Length + EditorGUIUtility.singleLineHeight * .5f;

            return pHeight;
        }
    }
}