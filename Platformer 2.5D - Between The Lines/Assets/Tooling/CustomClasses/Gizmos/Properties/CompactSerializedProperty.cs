#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tooling.GizmoShapes.Properties
{
    [Serializable]
    public class CompactSerializedProperty<T> : BaseCompactSerializedProperty
    {
        [SerializeReference] public T property;

        public override void Init()
        {
            propertyType = GetTypeName(typeof(T));
            UpdateSerializedProperty();
        }

        string GetTypeName(Type pType)
        {
            return pType switch
            {
                Type lType when lType == typeof(int) => "int",
                Type lType when lType == typeof(float) => "float",
                Type lType when lType == typeof(double) => "double",
                Type lType when lType == typeof(bool) => "bool",
                Type lType when lType == typeof(string) => "string",
                _ => pType.Name
            };
        }

        public override void UpdateSerializedProperty()
        {
            if(component != null)
            {
                int lIteration = propertyName.Count(f => f == '.') + 1;

                SerializedObject lSerializedObject = new SerializedObject(component);
                if (lIteration > 1)
                {
                    string[] lNameArray = propertyName.Split('.');
                    SerializedProperty lSerializedProperty = lSerializedObject.FindProperty(lNameArray[0]);

                    for (int i = 1; i < lIteration; i++)
                    {
                        serializedProperty = lSerializedProperty.FindPropertyRelative(lNameArray[i]);
                    }
                }
                else serializedProperty = lSerializedObject.FindProperty(propertyName);
            }
        }
    }
}
#endif