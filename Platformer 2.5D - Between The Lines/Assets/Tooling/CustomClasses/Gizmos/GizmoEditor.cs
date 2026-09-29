#if UNITY_EDITOR
using System.Drawing;
using UnityEditor;
using UnityEngine;

namespace Tooling.GizmoShapes
{
    [CustomEditor(typeof(Gizmo))]
    internal class GizmoEditor : Editor
    {
        private const int GIZMO_LENGTH = 5;
        private const int HANDLE_LENGTH = 6;

        private Gizmo _Gizmo;

        private string _ShapeName = "null";

        private void Awake()
        {
            _Gizmo = (Gizmo)target;
            //UpdateGizmoName(); TODO Gizmo / Handle
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ShowShapeButton();
        }

        private void OnSceneGUI()
        {
            if (!_Gizmo.isGizmo) _Gizmo.DrawGizmoAndHandle();
        }

        private void ShowShapeButton()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(_ShapeName);
            if (GUILayout.Button("Change Shape"))
            {
                ShowMenu();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void UpdateName(int pLength)
        {
            string lName = _Gizmo.shape.GetType().Name;
            _ShapeName = _Gizmo.shape == null ? "null" : lName.Substring(pLength);
        }

        private void ShowMenu()
        {
            GenericMenu lMenu = new GenericMenu();

            //Gizmos
            lMenu.AddItem(new GUIContent("Gizmo/Shape/Cube"), false, () => { _Gizmo.shape = new GizmoCube(); SetToGizmo(); });
            lMenu.AddItem(new GUIContent("Gizmo/Shape/Line"), false, () => {_Gizmo.shape = new GizmoLine(); SetToGizmo(); });
            lMenu.AddItem(new GUIContent("Gizmo/Shape/Mesh"), false, () => {_Gizmo.shape = new GizmoMesh(); SetToGizmo(); });
            lMenu.AddItem(new GUIContent("Gizmo/Shape/Ray"), false, () => {_Gizmo.shape = new GizmoRay(); SetToGizmo(); });
            lMenu.AddItem(new GUIContent("Gizmo/Shape/Sphere"), false, () => {_Gizmo.shape = new GizmoSphere(); SetToGizmo(); });

            lMenu.AddItem(new GUIContent("Gizmo/Wire/Cube"), false, () => {_Gizmo.shape = new GizmoWireCube(); SetToGizmo(); });
            lMenu.AddItem(new GUIContent("Gizmo/Wire/Sphere"), false, () => {_Gizmo.shape = new GizmoWireSphere(); SetToGizmo(); });
            lMenu.AddItem(new GUIContent("Gizmo/Wire/Mesh"), false, () => {_Gizmo.shape = new GizmoWireMesh(); SetToGizmo(); });
            lMenu.AddItem(new GUIContent("Gizmo/Wire/Frustum"), false, () => { _Gizmo.shape = new GizmoFrustum(); SetToGizmo(); });

            lMenu.AddItem(new GUIContent("Gizmo/Other/Icon"), false, () => {_Gizmo.shape = new GizmoIcon(); SetToGizmo(); });
            lMenu.AddItem(new GUIContent("Gizmo/Other/GUI Texture"), false, () => {_Gizmo.shape = new GizmoGUITexture(); SetToGizmo(); });
            lMenu.AddItem(new GUIContent("Gizmo/Other/GUI Texture"), false, () => {_Gizmo.shape = new GizmoGUITexture(); SetToGizmo(); });

            //Handle
            lMenu.AddItem(new GUIContent("Handle/Arrow/Multiple"), false, () => {_Gizmo.shape = new HandleArrowPolygon(); SetToHandle(); });

            lMenu.ShowAsContext();
        }

        private void SetToGizmo()
        {
            UpdateName(GIZMO_LENGTH);
            _Gizmo.isGizmo = true;
        }

        private void SetToHandle()
        {
            UpdateName(HANDLE_LENGTH);
            _Gizmo.isGizmo = false;
        }
    }
}
#endif