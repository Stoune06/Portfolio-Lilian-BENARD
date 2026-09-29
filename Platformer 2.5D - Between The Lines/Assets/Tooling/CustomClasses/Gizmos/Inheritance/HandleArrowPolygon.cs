#if UNITY_EDITOR
using System;
using Tooling.GizmoShapes.Properties;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Tooling.GizmoShapes
{
    internal class HandleArrowPolygon : GizmoShape
    {
        [SerializeField] public CompactSerializedProperty<Vector3> size;

        public override void Init(Transform pTransform)
        {
            size.Init();
            ready = true;
        }

        public override void Update(Transform pTransform)
        {
            size.UpdateSerializedProperty();
            SetAction(pTransform);
        }

        protected override void SetAction(Transform pTransform) //TODO beautifuller
        {
            if (size.useSerialized)
                m_Action = () =>
                {
                    EditorGUI.BeginChangeCheck();

                    Vector3 lNew = Handles.PositionHandle(
                        pTransform.TransformPoint(size.serializedProperty.vector3Value),
                        Quaternion.Euler(0f, 90f, 0f)
                    );

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(pTransform.gameObject, "Move Handle Arrow Polygon");
                        size.serializedProperty.vector3Value = pTransform.InverseTransformPoint(lNew);
                        size.UpdateSerializedObject();
                    }
                };
            else
                m_Action = () =>
                {

                    EditorGUI.BeginChangeCheck();

                    Vector3 lNew = Handles.PositionHandle(
                        pTransform.TransformPoint(size.property),
                        Quaternion.Euler(0f, 90f, 0f)
                    );

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(pTransform.gameObject, "Move Handle Arrow Polygon");
                        size.property = pTransform.InverseTransformPoint(lNew);
                    }
                };
        }
    }
}
#endif