#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Tooling.GizmoShapes.Properties
{
    [Serializable]
    public abstract class BaseCompactSerializedProperty
    {
        public const string TEMPLATE_PROPERTY = "property";

        [HideInInspector] public GameObject gameObject;
        [HideInInspector] public Component component;

        [HideInInspector] public string propertyName = "null";
        [HideInInspector] public string propertyType = "float";

        public SerializedProperty serializedProperty;
        [HideInInspector] public bool useSerialized = true;

        public abstract void Init();
        public abstract void UpdateSerializedProperty();
        public void UpdateSerializedObject() => serializedProperty.serializedObject.ApplyModifiedProperties();
    }
}
#endif