using UnityEditor;
using UnityEngine;

namespace Tooling.Attributes.Editors
{
    [CustomPropertyDrawer(typeof(ReadOnly))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pPosition, SerializedProperty pProperty, GUIContent lLabel)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(pPosition, pProperty, lLabel);
            GUI.enabled = true;
        }
    }
}