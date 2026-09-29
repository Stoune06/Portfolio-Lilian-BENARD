using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Editors.History.Utils;

namespace Editors.History
{
    internal class SearchWindow : BaseDisplayButtonWindow<LoadableGUIObject>
    {
        private Vector2 _ScrollPos = new Vector2();

        private bool _CloseIfSelected = false,
            _ShowEntirePath = false,
            _OpenIfSelected = true;

        private bool _IsShowingPreview = true;
        private bool LastShowingPreview = true;

        private string _AssetTypeString = "Default";

        private const string DEFAULT_PATH = "Assets";
        private string[] _FoldersToSearch = new string[1] { DEFAULT_PATH };

        private string _SearchedItemTextField = string.Empty;

        public event System.Action<LoadableGUIObject[]> onObjectSelected;

        string _ActualSearchedItem = string.Empty;

        private float _IconSize = 82f;

        private static Texture2D _LoopTexture;

        private static GUIStyle _LabelStyle;
        private GUIStyle _ButtonStyle;

        private List<LoadableGUIObject> _DisplayLoadableList = new List<LoadableGUIObject>();

        public static SearchWindow Open(Data pHistoryData, bool pCloseIfChosen = false, bool pShowEntirePath = false, bool pOpenIfSelected = true)
        {
            SearchWindow lWindow = GetWindow<SearchWindow>(pHistoryData.TypeName);
            lWindow.titleContent = new GUIContent($"Search/{pHistoryData.TypeName}", pHistoryData.logo);

            lWindow._AssetTypeString = T_SEARCH + pHistoryData.TypeName.ToLower();
            lWindow.SetWindow(pCloseIfChosen, pShowEntirePath, pOpenIfSelected);

            SetLabelStyle();
            _LoopTexture = (Texture2D)EditorGUIUtility.IconContent(SEARCH_ICON).image;

            lWindow.SetButtonStyle();
            lWindow.LoadObject();
            lWindow.Show();

            return lWindow;
        }

        private void SetButtonStyle()
        {
            _ButtonStyle = new GUIStyle(GUI.skin.button); 
            //_ButtonStyle.normal.background = Texture2D.redTexture; //Only Work on Windows10
            _ButtonStyle.normal.textColor = Color.white;
        }

        private void SetWindow(bool pCloseIfChosen, bool pShowEntirePath, bool pOpenIfSelected)
        {
            _OpenIfSelected = pOpenIfSelected;
            _CloseIfSelected = pCloseIfChosen;
            _ShowEntirePath = pShowEntirePath;
        }

        private static void SetLabelStyle()
        {
            _LabelStyle = new GUIStyle(EditorStyles.label);
            _LabelStyle.alignment = TextAnchor.MiddleCenter;
            _LabelStyle.fontSize = 10;
        }

        private void OnGUI()
        {
            SetSearchString();
            ShowUI();
            DisplayButton();

            if (_ActualSearchedItem != _SearchedItemTextField)
            {
                UpdateSearchedObject();
                Repaint();
            }
            if(LastShowingPreview != _IsShowingPreview)
            {
                LastShowingPreview = _IsShowingPreview;
                ShowType lShowType = _IsShowingPreview ? ShowType.Preview : ShowType.Thumbnail;
                for (int i = 0; i < m_LoadableObjectArray.Length; i++)
                {
                    m_LoadableObjectArray[i].UpdateTexture(lShowType);
                }
            }

            ListenEnter();
        }

        private void SetSearchString()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(_LoopTexture, GUILayout.Width(20f), GUILayout.Height(20f));
            GUI.SetNextControlName(m_GuiName);
            _SearchedItemTextField = GUILayout.TextField(_SearchedItemTextField);
            FocusControl();

            GUILayout.EndHorizontal();
        }

        private void ShowUI()
        {
            GUILayout.BeginHorizontal();
            _IconSize = EditorGUILayout.Slider(_IconSize, 32f, 128f);
            _ShowEntirePath = GUILayout.Toggle(_ShowEntirePath, "ShowEntirePath");
            _IsShowingPreview = GUILayout.Toggle(_IsShowingPreview, "Show Preview");
            GUILayout.EndHorizontal();
        }

        private void DisplayButton()
        {
            LoadableGUIObject lObject;
            Rect lRect;
            bool lBool;

            GUILayout.Space(10);

            _ScrollPos = GUILayout.BeginScrollView(_ScrollPos);
            int lColumn = Mathf.FloorToInt(position.width / (_IconSize + 4));

            for (int i = 0; i < _DisplayLoadableList.Count; i += lColumn)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < lColumn; j++)
                {
                    int lIndex = i + j;
                    if (lIndex >= _DisplayLoadableList.Count) continue;
                    lObject = _DisplayLoadableList[lIndex];

                    lBool = i + j == 0 && !m_FolderSelectable.IsSelecting || m_FolderSelectable.SelectList.Contains(lObject);

                    GUILayout.BeginVertical(GUILayout.Width(_IconSize));

                    lRect = GUILayoutUtility.GetRect(lObject.Content, GUI.skin.button, GUILayout.Width(_IconSize), GUILayout.Height(_IconSize));

                    DrawBoxWithIndepedantTexture(lRect, lObject.TargetObject, lObject.Texture, lBool ? GUI.skin.settings.selectionColor : Color.white);
                    ManageEventOnRect(lObject, lRect);

                    GUILayout.Label(_ShowEntirePath ? lObject.Path : lObject.Name, _LabelStyle, GUILayout.Width(_IconSize));

                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
        protected void ListenEnter()
        {
            if (Event.current.keyCode == KeyCode.Return)
            {
                if (_DisplayLoadableList.Count > 0) HandleCondition(_DisplayLoadableList[0]);
                Event.current.Use();
            }
        }

        protected override void OnControlClick(LoadableGUIObject pObject) => m_FolderSelectable.TryAddByControl(pObject);

        protected override void OnShiftClick(LoadableGUIObject pObject) => m_FolderSelectable.TryAddByShift(pObject, m_LoadableObjectArray);
        protected override void OnClick(LoadableGUIObject pObject)
        {
            HandleCondition(pObject);
        }

        protected override void OnLeftClick(LoadableGUIObject pObject)
        {
            ButtonGenericMenu(pObject).ShowAsContext();
            m_FolderSelectable.ClearSelectedList();
        }

        protected override void OnMouseDrag(LoadableGUIObject pObject)
        {
            DragAndDrop.PrepareStartDrag();

            if (m_FolderSelectable.IsSelecting)
            {
                Object[] lObjectArray = new Object[m_FolderSelectable.SelectList.Count];
                string[] lPathArray = new string[m_FolderSelectable.SelectList.Count];
                for (int i = 0; i < lObjectArray.Length; i++)
                {
                    lObjectArray[i] = m_FolderSelectable.SelectList[i].TargetObject;
                    lPathArray[i] = m_FolderSelectable.SelectList[i].Path;
                }

                DragAndDrop.objectReferences = lObjectArray;
                DragAndDrop.paths = lPathArray;
            }
            else
            {
                DragAndDrop.objectReferences = new Object[] { pObject.TargetObject };
                string lAssetPath = pObject.Path;
                DragAndDrop.paths = new string[] { lAssetPath };
            }

            DragAndDrop.StartDrag($"Dragging");
        }


        private void HandleCondition(LoadableGUIObject pObject)
        {
            if (_OpenIfSelected) EditorGUIUtility.PingObject(pObject.TargetObject);
            onObjectSelected?.Invoke(m_FolderSelectable.IsSelecting ? m_FolderSelectable.SelectList.ToArray() : new LoadableGUIObject[1] { pObject });

            TabBagWindow.UpdateAndRepaint();
            if (_CloseIfSelected) Close();
        }

        private void DrawBoxWithIndepedantTexture(Rect pRect, Object pObject, Texture2D pTexture, Color pColor)
        {
            GUI.color = pColor;
            GUI.Box(pRect, GUIContent.none, _ButtonStyle);
            GUI.color = Color.white;

            GUI.DrawTexture(pRect, pTexture, ScaleMode.ScaleToFit, true);
        }

        private void LoadObject()
        {
            _DisplayLoadableList.Clear();
            m_FolderSelectable.ClearSelectedList();
            LoadableGUIObject lLoadableObject;
            ShowType lShowType = _IsShowingPreview ? ShowType.Preview : ShowType.Thumbnail;
            string[] lGUIDArray = AssetDatabase.FindAssets(_AssetTypeString, _FoldersToSearch);

            for (int i = 0; i < lGUIDArray.Length; i++)
            {
                lLoadableObject = new LoadableGUIObject(lGUIDArray[i], lShowType);
                if (!TabBagWindow.LastInstance.CurrentData.GUIDList.Contains(lLoadableObject.Guid) 
                    && lLoadableObject.TargetObject.GetType().Name == TabBagWindow.LastInstance.CurrentData.TypeName)
                    _DisplayLoadableList.Add(lLoadableObject);
            }

            m_LoadableObjectArray = _DisplayLoadableList.ToArray();
        }

        private void UpdateSearchedObject()
        {
            if (_SearchedItemTextField.Contains(_ActualSearchedItem, System.StringComparison.OrdinalIgnoreCase))
            {
                int lIteration = _DisplayLoadableList.Count - 1;
                for (int i = lIteration; i >= 0; i--)
                {
                    if (!_DisplayLoadableList[i].Path.Contains(_SearchedItemTextField, System.StringComparison.OrdinalIgnoreCase))
                        _DisplayLoadableList.RemoveAt(i);
                }
            }
            else
            {
                _DisplayLoadableList.Clear();
                LoadableGUIObject lObject;

                for (int i = 0; i < m_LoadableObjectArray.Length; i++)
                {
                    lObject = m_LoadableObjectArray[i];
                    if (lObject.Path.Contains(_SearchedItemTextField, System.StringComparison.OrdinalIgnoreCase))
                        _DisplayLoadableList.Add(lObject);
                }
            }

            m_FolderSelectable.ClearSelectedList();

            _ActualSearchedItem = _SearchedItemTextField;

        }
    }
}