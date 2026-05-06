#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Tooling.GizmoShapes.Properties
{
    [CustomPropertyDrawer(typeof(CompactSerializedProperty<>))]
    public class CompactSerializedPropertyDrawer : PropertyDrawer
    {
        public const char NOT_AUTHORIZE = '<';

        private SerializedProperty _SerializedGameObject;
        private SerializedProperty _StringProperty;
        private SerializedProperty _ComponentProperty;
        private SerializedProperty _TypeProperty;
        private SerializedProperty _TemplateProperty;
        private SerializedProperty _BoolProperty;

        private GameObject _GameObject;
        private bool waitInit = true;

        private GUIContent _EmptyContent = new GUIContent("");

        private void Awake()
        {
            UpdateProperty();
        }

        private void Init(SerializedProperty pProperty)
        {
            if (waitInit)
            {
                _SerializedGameObject = pProperty.FindPropertyRelative(nameof(BaseCompactSerializedProperty.gameObject));
                _StringProperty = pProperty.FindPropertyRelative(nameof(BaseCompactSerializedProperty.propertyName));
                _ComponentProperty = pProperty.FindPropertyRelative(nameof(BaseCompactSerializedProperty.component));
                _TypeProperty = pProperty.FindPropertyRelative(nameof(BaseCompactSerializedProperty.propertyType));
                _TemplateProperty = pProperty.FindPropertyRelative(BaseCompactSerializedProperty.TEMPLATE_PROPERTY);
                _BoolProperty = pProperty.FindPropertyRelative(nameof(BaseCompactSerializedProperty.useSerialized));

                _GameObject = (GameObject)_SerializedGameObject.objectReferenceValue;
            }
        }

        public override void OnGUI(Rect pPosition, SerializedProperty pProperty, GUIContent pLabel)
        {
            Init(pProperty);


            if (_BoolProperty.boolValue) ShowSerializedProperty(pProperty);
            else ShowProperty(pProperty);
        }

        private void ShowSerializedProperty(SerializedProperty pProperty)
        {
            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(_BoolProperty, new GUIContent(_BoolProperty.name));
            EditorGUILayout.PropertyField(_SerializedGameObject, _EmptyContent);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = false;
            EditorGUILayout.PropertyField(_ComponentProperty, _EmptyContent);
            GUI.enabled = true;

            ButtonGenericMenu();

            GUILayout.EndHorizontal();
        }

        private void ShowProperty(SerializedProperty pProperty)
        {
            EditorGUILayout.PropertyField(_BoolProperty, new GUIContent(_BoolProperty.name));
            EditorGUILayout.PropertyField(_TemplateProperty, new GUIContent(pProperty.name));
        }

        private void UpdateProperty()
        {

        }

        private void ButtonGenericMenu()
        {
            if (GUILayout.Button(_StringProperty.stringValue))
            {
                DisplayGenericMenu(_StringProperty, _ComponentProperty);
            }
        }

        private void DisplayGenericMenu(SerializedProperty pStringProperty, SerializedProperty lComponentProperty) //TODO if type == Quaternion "m_LocalEulerAnglesHint"
        {
            if (_GameObject != null)
            {
                Component[] lComponentArray = _GameObject.GetComponents<Component>();
                GenericMenu lMenu = new GenericMenu();
                lMenu.allowDuplicateNames = true;
                string lParent = string.Empty;

                SerializedObject lSerializedTarget = pStringProperty.serializedObject;

                SerializedObject lSerializedObject;
                SerializedProperty lSerializedProperty;

                string lVisualString = string.Empty;
                string lTrueParentString = string.Empty;

                foreach (Component item in lComponentArray)
                {
                    lSerializedObject = new SerializedObject(item);
                    lSerializedProperty = lSerializedObject.GetIterator();

                    while (lSerializedProperty.NextVisible(true))
                    {
                        string lTrueString = string.Empty;
                        if (lSerializedProperty.depth > 0)
                        {
                            lVisualString = $"{lParent}/{lSerializedProperty.name}";
                            lTrueString = $"{lTrueParentString}.{lSerializedProperty.name}";
                        }
                        else
                        {
                            lParent = $"{item.GetType().Name}/{lSerializedProperty.name}";
                            lTrueParentString = lSerializedProperty.name;
                            lVisualString = lParent;
                            lTrueString = lTrueParentString;
                        }
                        if (!lVisualString.Contains(NOT_AUTHORIZE) && lSerializedProperty.type == _TypeProperty.stringValue)
                        {
                            lMenu.AddItem(new GUIContent(lVisualString), false, () =>
                            {
                                pStringProperty.stringValue = lTrueString;

                                lComponentProperty.objectReferenceValue = item;
                                lSerializedTarget.ApplyModifiedProperties();
                            });
                        }
                    }
                }
                lMenu.ShowAsContext();
            }
            else EditorUtility.DisplayDialog("Null Game Object", "Please do select a Game Object beforehand", "Ok");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0f;
    }
}
#endif