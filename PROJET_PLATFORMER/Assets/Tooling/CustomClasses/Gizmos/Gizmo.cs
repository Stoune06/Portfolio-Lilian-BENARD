#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Tooling.GizmoShapes
{
    [Icon("Assets/Tooling/CustomClasses/Gizmos/Gizmo.png"), ExecuteInEditMode]
    internal sealed class Gizmo : MonoBehaviour
    {
        [SerializeField] public Color color = new Color(1f, 0f, 0f, 0.33f);
        [SerializeField] public bool show = true;
        [HideInInspector] public bool isGizmo = true;

        [SerializeReference, SerializeField] internal GizmoShape shape = new GizmoCube();

        [HideInInspector, SerializeField] internal MonoBehaviour targetedMonoBehaviour;

        [NonSerialized] public bool updateOnNextFrame = false;
        [NonSerialized] public bool isWaitingInit = true;

        private void Awake()
        {
            shape.Init(gameObject.transform);
            isWaitingInit = false;
        }

        private void OnDrawGizmosSelected()
        {
            if (isGizmo) DrawGizmoAndHandle();
        }

        public void DrawGizmoAndHandle()
        {
            if (show)
            {
                if (isWaitingInit)
                {
                    shape.Init(gameObject.transform);
                    isWaitingInit = false;
                }

                if (shape.ready)
                    shape.DrawShape(color);

                if (updateOnNextFrame)
                {
                    try
                    {
                        shape.Update(gameObject.transform);
                    }
                    catch { }
                    //updateOnNextFrame = false; //Todo opti
                }

                Handles.color = Color.magenta;
                transform.position = Handles.PositionHandle(transform.position, Quaternion.identity);
            }
        }

        private void OnValidate()
        {
            updateOnNextFrame = true;
            isWaitingInit = true;
        }
    }
}
#endif