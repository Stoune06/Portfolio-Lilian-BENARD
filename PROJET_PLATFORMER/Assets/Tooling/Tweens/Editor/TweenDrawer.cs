#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens
{
    [CustomPropertyDrawer(typeof(TweenProperty), true)]
    public class TweenDrawer : PropertyDrawer
    {
        private const string LOOP = "loop", LOOP_REVERSE = "loopThenReverse", IS_A_LOOP_ACTIVATED = "IsALoopActivated", LOOP_REVERSE_ONCE = "loopThenReverseOnce";
        private const float MARGIN = 16f;

        public override void OnGUI(Rect pPosition, SerializedProperty pProperty, GUIContent pLabel)
        {
            EditorGUI.BeginProperty(pPosition, pLabel, pProperty);
            float pY = pPosition.y;

            Rect pStartRect = new Rect(pPosition.x, pY, pPosition.width, EditorGUI.GetPropertyHeight(pProperty, true));
            EditorGUI.PropertyField(pStartRect, pProperty, new GUIContent(pProperty.displayName), true);
            pY += pStartRect.height;

            if (pProperty.isExpanded)
            {
                SerializedProperty pLoopSerialized = pProperty.FindPropertyRelative(LOOP);
                SerializedProperty pReversedLoopSerialized = pProperty.FindPropertyRelative(LOOP_REVERSE);
                SerializedProperty pReverseOnceSerialized = pProperty.FindPropertyRelative(LOOP_REVERSE_ONCE);
                SerializedProperty pLoopActivated = pProperty.FindPropertyRelative(IS_A_LOOP_ACTIVATED);

                Rect pRect;

                pRect = new Rect(pPosition.x + EditorGUI.indentLevel + MARGIN, pY, pPosition.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(pRect, pReverseOnceSerialized);

                if (pReverseOnceSerialized.boolValue)
                {
                    pReversedLoopSerialized.boolValue = false;
                    pLoopSerialized.boolValue = false;
                }

                pY += EditorGUIUtility.singleLineHeight;
                pRect = new Rect(pPosition.x + EditorGUI.indentLevel + MARGIN, pY, pPosition.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(pRect, pLoopSerialized);

                if ( pLoopSerialized.boolValue)
                {
                    pReverseOnceSerialized.boolValue = false;
                    pReversedLoopSerialized.boolValue = false;
                }

                pY += EditorGUIUtility.singleLineHeight;
                pRect = new Rect(pPosition.x + MARGIN, pY, pPosition.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(pRect, pReversedLoopSerialized);

                if (pReversedLoopSerialized.boolValue)
                {
                    pReverseOnceSerialized.boolValue = false;
                    pLoopSerialized.boolValue = false;
                }

                pLoopActivated.boolValue = pLoopSerialized.boolValue || pReversedLoopSerialized.boolValue || pReverseOnceSerialized.boolValue;

            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty pProperty, GUIContent pLabel)
        {
            float lHeight = EditorGUI.GetPropertyHeight(pProperty, true);
            if (pProperty.isExpanded)
                lHeight += EditorGUIUtility.singleLineHeight * 3f;
            return lHeight;
        }
    }
}
#endif