//Clement PERSYN

using Com.IsartDigital.HealerSurvivor.InfluenceMap.Personality;
using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.InfluenceMap.DebugTools
{
    public class MapWindow : EditorWindow
    {
        private const string WINDOW_NAME = "Map";

        private InfluenceGrid _InfluenceGrid;
        private MapColorParameter _ColorParameters;
        private InfluencePersonality _Personality;
        private InfluenceCharacter _InfluChara;
        private bool _IsDrawing;

        private bool _IsTracking = true;

        [MenuItem("Tools/" + WINDOW_NAME)]
        public static void ShoWindow()
        {
            MapWindow lWindow = GetWindow<MapWindow>();
            lWindow.titleContent = new GUIContent(WINDOW_NAME);
        }

        private void OnEnable()
        {
             
            FindParameters();
        }

        private void FindGrid()
        {
            InfluenceGrid lFoundGrid = Object.FindAnyObjectByType<InfluenceGrid>();

            if (lFoundGrid != null) _InfluenceGrid = lFoundGrid;
        }

        private void FindParameters()
        {
            string[] lGuids = AssetDatabase.FindAssets("t:MapColorParameter");
            if (lGuids.Length > 0)
            {
                string lPath = AssetDatabase.GUIDToAssetPath(lGuids[0]);
                _ColorParameters = AssetDatabase.LoadAssetAtPath<MapColorParameter>(lPath);
                return;
            }

            Debug.Log("Aucun paramètre trouvé");
        }

        private void StartDrawing()
        {
            _InfluenceGrid.CreateCells();
            SceneView.duringSceneGui += DrawCells;
        }

        private void StopDrawing()
        {
            SceneView.duringSceneGui -= DrawCells;
            _IsDrawing = false;
        }

        private void OnGUI()
        {
            _InfluenceGrid = (InfluenceGrid)EditorGUILayout.ObjectField("Glissez la map ", _InfluenceGrid, typeof(InfluenceGrid), true);

            _IsTracking = EditorGUILayout.Toggle("Track heros", _IsTracking);
            if (_IsTracking)
            {
                if (Selection.activeTransform != null && Selection.activeTransform.gameObject.TryGetComponent<InfluenceCharacter>(out _InfluChara))
                    _Personality = _InfluChara.personality;
                else
                {
                    _InfluChara = null;
                    _Personality = null;
                }
            }
            else
            {
                _InfluChara = (InfluenceCharacter)EditorGUILayout.ObjectField("Glissez le player à suivre ", _InfluChara, typeof(InfluenceCharacter), true);
                _Personality = (InfluencePersonality)EditorGUILayout.ObjectField("Glissez la personalité ", _Personality, typeof(InfluencePersonality), false);
            }

            if (_InfluenceGrid == null || _ColorParameters == null || _Personality == null)
            {
                if (_InfluenceGrid == null) FindGrid();
                if (_InfluenceGrid == null) EditorGUILayout.HelpBox("Aucune InfluenceGrid assignée.", MessageType.Info);
                if (_Personality == null && !_IsTracking) EditorGUILayout.HelpBox("Aucun InfluencePersonality assignée.", MessageType.Info);
                if (_ColorParameters == null && !_IsTracking) EditorGUILayout.HelpBox("Aucun SO MapColorParameter trouvé.", MessageType.Warning);

                //if (_IsDrawing) StopDrawing();
            }

            EditorGUILayout.Space();

            bool lToggle = EditorGUILayout.Toggle("Show Influence", _IsDrawing);

            if (lToggle && !_IsDrawing) StartDrawing();
            else if (!lToggle && _IsDrawing) StopDrawing();

            _IsDrawing = lToggle;
            if (!_IsDrawing || _InfluenceGrid == null || _ColorParameters == null || _Personality == null) return;

            if (_IsDrawing && GUILayout.Button("Update Cells")) _InfluenceGrid.CreateCells();

            if (_IsDrawing && _IsTracking && _InfluChara != null) Editor.CreateEditor(_InfluChara.personality).OnInspectorGUI();

            Editor.CreateEditor(_InfluenceGrid).OnInspectorGUI();


            EditorGUILayout.Space();
            EditorGUILayout.Space();

            Editor.CreateEditor(_ColorParameters).OnInspectorGUI();
        }

        private void DrawCells(SceneView pView)
        {
            if (_InfluenceGrid == null || _ColorParameters == null || _Personality == null || _InfluenceGrid.cells == null) return;

            InfluenceCell lCell;
            InfluenceCell lBestCell = null;

            Color lColor;
            float lT;

            if (_InfluChara != null)
            {
                lBestCell = InfluenceManager.GetBestCell(_InfluChara, _Personality);
                _InfluenceGrid.AddLocalInflu(_InfluChara.transform.position, - _InfluChara.myEstimateInfluence, _InfluChara.radius, _InfluChara.influType);
                _InfluenceGrid.AddLocalInflu(_InfluChara.transform.position, (1f / InfluenceCell.DECAY_FACTOR) + (1f / InfluenceCharacter.UPDATE_FREQ), InfluenceTypeEnum.ProximityFactor);
            }
                

            for (int lX = 0; lX < _InfluenceGrid.cells.GetLength(0); lX++)
            {
                for (int lY = 0; lY < _InfluenceGrid.cells.GetLength(1); lY++)
                {
                    lCell = _InfluenceGrid.cells[lX, lY];

                    lT = Mathf.InverseLerp(_ColorParameters.minValue, _ColorParameters.maxValue, lCell.GetInfluence(_Personality, true));
                    if (_InfluChara != null && lBestCell == lCell)
                        lColor = Color.cyan;
                    else
                        lColor = _ColorParameters.influenceGradient.Evaluate(lT);
                    lColor.a = _ColorParameters.baseAlpha;

                    CreateSquare(lCell.globalPosition, lCell.size, lColor);
                }
            }
            _InfluenceGrid.ResetLocalInflu();
            pView.Repaint();       
        }

        private void CreateSquare(Vector3 pPosition, Vector2 pSize, Color pColor)
        {
            Vector3[] lCorners = new Vector3[4];
            Handles.color = pColor;

            float lHalfX = pSize.x * 0.5f;
            float lHalfY = pSize.y * 0.5f;

            lCorners[0] = new Vector3(pPosition.x - lHalfX, pPosition.y, pPosition.z - lHalfY);
            lCorners[1] = new Vector3(pPosition.x + lHalfX, pPosition.y, pPosition.z - lHalfY);
            lCorners[2] = new Vector3(pPosition.x + lHalfX, pPosition.y, pPosition.z + lHalfY);
            lCorners[3] = new Vector3(pPosition.x - lHalfX, pPosition.y, pPosition.z + lHalfY);

            Handles.DrawAAConvexPolygon(lCorners);

            if (!_ColorParameters.showContour) return;

            Handles.color = Color.black;
            Handles.DrawPolyLine(lCorners);
            Handles.DrawLine(lCorners[3], lCorners[0]);
        }

        private void OnDisable()
        {
            StopDrawing();
        }

        private void OnSelectionChange()
        {
            if (_IsTracking) Repaint();
        }
    }
}
