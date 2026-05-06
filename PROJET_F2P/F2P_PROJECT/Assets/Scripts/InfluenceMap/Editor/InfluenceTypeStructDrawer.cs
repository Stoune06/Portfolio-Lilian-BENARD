using Com.IsartDigital.HealerSurvivor.InfluenceMap.Personality;
using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.InfluenceMap
{
    [CustomPropertyDrawer(typeof(InfluenceTypeStruct))]
    public class InfluenceTypeStructDrawer : PropertyDrawer
    {
        private const float LINE_HEIGHT = 20;

        public override void OnGUI(Rect pPosition, SerializedProperty pProperty, GUIContent pLabel)
        {
            Rect lRect = new Rect(pPosition.x, pPosition.y, pPosition.width, LINE_HEIGHT);

            string lName = pProperty.FindPropertyRelative("name").stringValue;
 
            pProperty.FindPropertyRelative("value").floatValue = EditorGUI.FloatField(lRect,lName, pProperty.FindPropertyRelative("value").floatValue);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return LINE_HEIGHT;
        }
    }
}