using Editors.History.Parameters;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using static Editors.History.Utils;

namespace Editors.History
{
    internal class TabBagWindow : BaseDisplayButtonWindow<string> //TODO CLEAN
    {
        private static List<TabBagWindow> _TabBagList = new List<TabBagWindow>();
        public bool IsGenericMenuOpen { get; private set; } = false;

        private static Object[] _ClipBoardedObject = null;
        private static Data _ClipBoardedData = null;

        private GUIContent[] _GuiContentArray;

        internal List<Data> DataList { get; private set; } = new List<Data>();
        internal Data CurrentData;

        public static TabBagWindow LastInstance { get; private set; }
        private Vector2 _ScrollPos = Vector2.zero;

        internal static HistoryParameter Parameter { get; private set; }
        internal static ParameterPack ParameterPack { get; private set; }

        private System.Func<LoadableGUIObject, Rect> GetButtonGUIRect;
        private System.Func<int, Data, Rect> GetTabGUIRect;

        private System.Action _BeginHV, _EndHV, _ShowTab, _ShowButton;
        internal Vector2 LastSize { get; private set; } = Vector3.zero;
        internal float TabVerticalSize { get; private set; }
        internal float ButtonVerticalSize { get; private set; }

        bool _IsVertical = false;

        static TabBagWindow()
        {
            System.Type lType = typeof(Editor).Assembly.GetType("UnityEditor.HostView");
            FieldInfo lField = lType.GetField("k_DockedMinSize", BindingFlags.Static | BindingFlags.NonPublic);
            lField!.SetValue(null, new Vector2(100f, ICON_SIZE));
        }

        [MenuItem("Window/History/Window")]
        public static TabBagWindow Open()
        {
            TabBagWindow lWindow = CreateInstance<TabBagWindow>();
            lWindow.Show();
            lWindow.titleContent = new GUIContent(HISTORY, GetIconContent(TAB_ICON));
            return lWindow;
        }

        public void CreateGUI()
        {
            LastInstance = this;
            SetFolder();

            LastSize = position.size;
            SetDataBag();
            CheckSize();

            _TabBagList.Add(this);
            EditorWindow.windowFocusChanged += () => { m_FolderSelectable.ClearSelectedList(); Repaint(); };
        }

        private void OnGUI()
        {
            if (LastSize != position.size) CheckSize();
            CheckDraggedObject();

            if (DataList.Count >= 0 && CurrentData != null)
            {
                ShowTabs();
                ShowHistory();
            }
            else
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(EditorGUIUtility.IconContent(BAG_ICON).image, GUILayoutSize(SIZE))) DisplayBagChoosingMenu();
                GUIStyle lGUIStyle = new GUIStyle(EditorStyles.boldLabel);

                lGUIStyle.fontSize = 25;
                GUILayout.Label("Drop Item To Start", lGUIStyle);
                GUILayout.EndHorizontal();
            }
            CheckEvent();
        }

        private void ShowHistory()
        {
            _ScrollPos = GUILayout.BeginScrollView(_ScrollPos);
            _BeginHV();

            int lIteration = m_LoadableObjectArray.Length - 1;
            LoadableGUIObject lLoadableObject;

            for (int i = lIteration; i >= 0; i--)
            {
                lLoadableObject = m_LoadableObjectArray[i];
                HistoryButton(lLoadableObject);
            }

            _EndHV();
            GUILayout.EndScrollView();
        }

        public void HistoryButton(LoadableGUIObject pLoadableObject)
        {
            if (pLoadableObject.TargetObject == null)
            {
                CurrentData.TryRemove(pLoadableObject.Guid);
                UpdateLoadableGUIObject();
                return;
            }

            Rect lRect = GetButtonGUIRect(pLoadableObject);

            if (m_FolderSelectable.IsSelecting && m_FolderSelectable.SelectList.Contains(pLoadableObject.Guid))
                GUI.backgroundColor = GUI.skin.settings.selectionColor;

            GUI.Box(lRect, pLoadableObject.Content, GUI.skin.button);

            if (GUI.backgroundColor == GUI.skin.settings.selectionColor)
            {
                Repaint();
                GUI.backgroundColor = Color.white;
            }

            ManageEventOnRect(pLoadableObject, lRect);
        }

        private Rect GetButtonHorizontalRect(LoadableGUIObject pLoadable)
            => GUILayoutUtility.GetRect(pLoadable.Content, EditorStyles.toolbarButton, GUILayoutSize(pLoadable.ChoppedName.Length * CHAR_SIZE + ICON_SIZE, SIZE));

        private Rect GetButtonVerticalRect(LoadableGUIObject pLoadable)
        {
            Rect lRect = GUILayoutUtility.GetRect(pLoadable.Content, EditorStyles.toolbarButton, GUILayoutSize(ButtonVerticalSize, SIZE));
            lRect.position += SIZE * 3f * Vector2.down;
            lRect.position += TabVerticalSize * Vector2.right;
            return lRect;
        }

        private Rect GetTabHorizontalRect(int lIteration, Data lData)
            => GUILayoutUtility.GetRect(new GUIContent(_GuiContentArray[lIteration]), EditorStyles.toolbarButton, GUILayout.Width((position.width - SIZE) / DataList.Count), GUILayout.Height(SIZE));

        private Rect GetTabVerticalRect(int lIteration, Data lData)
            => GUILayoutUtility.GetRect(new GUIContent(lData.logo), EditorStyles.toolbarButton, GUILayoutSize(TabVerticalSize, SIZE));

        private void ShowTabs()
        {
            _BeginHV();
            Data lData;
            Rect lTabRect;
            Event lEvent = Event.current;

            for (int i = 0; i < DataList.Count; i++)
            {
                lData = DataList[i];
                lTabRect = GetTabGUIRect(i, lData);

                if (lEvent.type == EventType.MouseDown && lTabRect.Contains(lEvent.mousePosition))
                {
                    if (lEvent.button == 0)
                    {
                        CurrentData = DataList[i];
                        UpdateLoadableGUIObject();
                        Parameter.CurrentTabNumber = i;
                        lEvent.Use();
                    }
                    else if (lEvent.button == 1)
                    {
                        TabGenericMenu(lData);
                        lEvent.Use();
                    }

                    m_FolderSelectable.ClearSelectedList();
                }
                GUI.Toggle(lTabRect, CurrentData == lData, _IsVertical ? new GUIContent(_GuiContentArray[i].image) : _GuiContentArray[i], EditorStyles.toolbarButton);
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent(BAG_ICON).image, GUILayoutSize(SIZE))) DisplayBagChoosingMenu();

            _EndHV();
        }

        protected override void OnControlClick(LoadableGUIObject pObject) => m_FolderSelectable.TryAddByControl(pObject.Guid);
        protected override void OnShiftClick(LoadableGUIObject pObject) => m_FolderSelectable.TryAddByShift(pObject.Guid, CurrentData.GUIDList);
        protected override void OnClick(LoadableGUIObject pObject)
        {
            AssetDatabase.OpenAsset(pObject.TargetObject);
            m_FolderSelectable.ClearSelectedList();
        }

        protected override void OnLeftClick(LoadableGUIObject pObject)
        {
            ButtonGenericMenu(pObject);
            if (!IsGenericMenuOpen) m_FolderSelectable.ClearSelectedList();
        }

        protected override void OnMouseDrag(LoadableGUIObject pObject)
        {
            DragAndDrop.PrepareStartDrag();

            if (m_FolderSelectable.IsSelecting)
            {
                DragAndDrop.objectReferences = GUIDSToGameObject(m_FolderSelectable.SelectList).ToArray();
                DragAndDrop.paths = GUIDSToPaths(m_FolderSelectable.SelectList).ToArray();
            }
            else
            {
                DragAndDrop.objectReferences = new Object[] { pObject.TargetObject };
                string lAssetPath = pObject.Path;
                DragAndDrop.paths = new string[] { lAssetPath };
            }

            DragAndDrop.StartDrag($"Dragging");
        }

        internal void SetDataBag()
        {
            if (Parameter.GetCurrentBagPath() == null)
                Parameter.SetCurrentBagPath(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(FOLDER_SEARCH, new string[] { DATA_PATH })[0]));

            UpdateData();
            SetTitleContent(Parameter.GetCurrentBagPath());
        }

        internal void SetDataBag(string pPath)
        {
            Parameter.SetCurrentBagPath(pPath);
            UpdateData();
            Parameter.CurrentTabNumber = 0;
            SetTitleContent(Parameter.GetCurrentBagPath());
            Repaint();
        }

        public void UpdateData()
        {
            AssetDatabase.Refresh();

            m_FolderSelectable.ClearSelectedList();
            DataList.Clear();

            string[] lPathArray = AssetDatabase.FindAssets(T_SEARCH + nameof(Data), new string[] { Parameter.GetCurrentBagPath() });
            _GuiContentArray = new GUIContent[lPathArray.Length];

            for (int i = 0; i < lPathArray.Length; i++)
            {
                Data lData;
                int lInt = i;
                lData = AssetDatabase.LoadAssetAtPath<Data>(AssetDatabase.GUIDToAssetPath(lPathArray[i]));
                DataList.Add(lData);

                _GuiContentArray[i] = new GUIContent(lData.tabName, lData.logo);
                lData.onValidate += () => { _GuiContentArray[lInt] = new GUIContent(lData.tabName, lData.logo); Repaint(); };
            }

            if (DataList.Count <= 0) CurrentData = null;
            else
            {
                CurrentData = Parameter.CurrentTabNumber > DataList.Count ? DataList[0] : DataList[Parameter.CurrentTabNumber];
                UpdateLoadableGUIObject();
            }
        }

        public void UpdateLoadableGUIObject()
        {
            m_LoadableObjectArray = new LoadableGUIObject[CurrentData.GUIDList.Count];
            for (int i = 0; i < CurrentData.GUIDList.Count; i++)
            {
                m_LoadableObjectArray[i] = new LoadableGUIObject(CurrentData.GUIDList[i], ParameterPack.showType);
            }
            Repaint();
        }

        private void DisplayBagChoosingMenu()
        {
            GenericMenu lMenu = new GenericMenu();
            string[] lPathArray = GetFoldersPathsInFolders(Parameter.savePath).ToArray();
            int pCharacterToIgnore = Parameter.savePath.Length;

            string lCurrentTabPath = Parameter.GetCurrentBagPath();

            for (int i = 0; i < lPathArray.Length; i++)
            {
                string lPath = lPathArray[i].Replace('\\', '/');

                if (lCurrentTabPath != lPath)
                {
                    lMenu.AddItem(new GUIContent("Open" + lPath.Substring(pCharacterToIgnore)), false, () => SetDataBag(lPath));
                    lMenu.AddItem(new GUIContent("Delete" + lPath.Substring(pCharacterToIgnore) + "/You sure ?"), false, () => DeleteDirectory(lPath));
                }
            }

            lMenu.AddSeparator(string.Empty);
            lMenu.AddItem(new GUIContent("Create New Bag"), false, () => BagMakerWindow.Open());
            lMenu.AddItem(new GUIContent("Force Update"), false, () => UpdateData());

            lMenu.ShowAsContext();
        }


        private void TabGenericMenu(Data pData)
        {
            GenericMenu lMenu = new GenericMenu();
            IsGenericMenuOpen = true;

            lMenu.AddItem(new GUIContent("Open Data"), false, () => AssetDatabase.OpenAsset(pData));

            lMenu.AddItem(new GUIContent($"Remove/{pData.tabName} From Unity Storage/ You Sure ?"), false, () => { DeleteFile(AssetDatabase.GetAssetPath(pData)); UpdateData(); });
            lMenu.AddItem(new GUIContent($"Remove/All Objects From {pData.tabName} History"), false, () => pData.RemoveAllPath());

            CopyPaste(lMenu, pData);
            lMenu.ShowAsContext();
        }

        protected virtual void ButtonGenericMenu(LoadableGUIObject pObject)
        {
            GenericMenu lMenu = new GenericMenu();
            IsGenericMenuOpen = true;

            if (!m_FolderSelectable.IsSelecting)
            {
                lMenu.AddItem(new GUIContent("Copy/Object"), false, () => _ClipBoardedObject = new Object[1] { pObject.TargetObject });
                lMenu.AddItem(new GUIContent("Copy/Path"), false, () => GUIUtility.systemCopyBuffer = pObject.Path);
                lMenu.AddItem(new GUIContent("Copy/User Path"), false, () => GUIUtility.systemCopyBuffer = Application.streamingAssetsPath + pObject.Path);

                int lResourcePath = pObject.Path.IndexOf(RESOURCES_SIGNATURE);
                if (lResourcePath < 0) lMenu.AddDisabledItem(new GUIContent("Copy/Resource Path"));
                else
                    lMenu.AddItem(new GUIContent("Copy/ResourcePath"), false,
                        () => GUIUtility.systemCopyBuffer = Path.ChangeExtension(pObject.Path.Substring(lResourcePath + RESOURCES_SIGNATURE.Length), null));

                lMenu.AddItem(new GUIContent("Copy/GUID"), false, () => GUIUtility.systemCopyBuffer = pObject.Guid);

                lMenu.AddSeparator(string.Empty);
                lMenu.AddItem(new GUIContent("Open At Path"), false, () => { EditorApplication.ExecuteMenuItem("Window/General/Project"); EditorGUIUtility.PingObject(pObject.TargetObject); });
                lMenu.AddItem(new GUIContent("Show In Explorer"), false, () => ShowInExplorer(pObject.Path));

                //lMenu.AddSeparator(string.Empty);
                //lMenu.AddItem(new GUIContent("Find Reference In Project"), false, () => SearchService.ShowWindow(new SearchContext(SearchService.CreateContext(new[] { "asset", "scene" }, $"ref:{pObject.Path}"))));

                lMenu.AddSeparator(string.Empty);
                lMenu.AddItem(new GUIContent("Remove From/Tab"), false, () => { CurrentData.TryRemove(pObject.Guid); UpdateLoadableGUIObject(); });
                lMenu.AddItem(new GUIContent("Remove From/Unity Project/ You Sure ?"), false, () => { CurrentData.TryRemove(pObject.Guid); DeleteFile(pObject.TargetObject); UpdateLoadableGUIObject(); });
            }
            else
            {
                lMenu.AddItem(new GUIContent("Copy/Object"), false, () => _ClipBoardedObject = GUIDSToGameObject(m_FolderSelectable.SelectList).ToArray());
                lMenu.AddDisabledItem(new GUIContent("Copy/Path"));
                lMenu.AddDisabledItem(new GUIContent("Copy/User Path"));
                lMenu.AddDisabledItem(new GUIContent("Copy/Resource Path"));
                lMenu.AddDisabledItem(new GUIContent("Copy/GUID"));

                lMenu.AddSeparator(string.Empty);
                lMenu.AddDisabledItem(new GUIContent("Open At Path"));
                lMenu.AddDisabledItem(new GUIContent("Show In Explorer"));

                lMenu.AddSeparator(string.Empty);
                lMenu.AddItem(new GUIContent("Remove From/Tab"), false, () => { CurrentData.TryRemoves(m_FolderSelectable.SelectList); m_FolderSelectable.ClearSelectedList(); UpdateLoadableGUIObject(); });

                lMenu.AddItem(new GUIContent("Remove From/Unity Project/ You Sure ?"), false, () =>
                {
                    foreach (string item in m_FolderSelectable.SelectList)
                    {
                        CurrentData.TryRemove(item);
                        DeleteFile(AssetDatabase.GUIDToAssetPath(item));
                    }
                    AssetDatabase.Refresh();
                    UpdateLoadableGUIObject();
                });
            }
            lMenu.ShowAsContext();
        }

        private void CheckDraggedObject()
        {
            Event lEvent = Event.current;
            Rect lRect = new Rect(0f, 0f, position.width, position.height);
            if (lEvent == null) return;

            switch (lEvent.type)
            {
                case EventType.DragUpdated:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    lEvent.Use();
                    break;

                case EventType.DragPerform:
                    DragAndDrop.AcceptDrag();
                    ManageDroppedObjects(DragAndDrop.objectReferences);
                    lEvent.Use();
                    break;
            }
        }

        private void ManageDroppedObjects(Object[] pObjectArray)
        {
            System.Type lType = pObjectArray[0].GetType();

            for (int i = 0; i < pObjectArray.Length; i++)
            {
                if (pObjectArray[i].GetType() != lType)
                {
                    EditorApplication.Beep();
                    EditorUtility.DisplayDialog("Type Issue", "Please select multiple object from the same Type", "Ok");
                    return;
                }
            }

            if (!Event.current.shift && CurrentData != null && lType.Name == CurrentData.TypeName)
            {
                for (int i = 0; i < pObjectArray.Length; i++)
                {
                    CurrentData.TryAdd(pObjectArray[i]);
                }
                UpdateAndRepaint();
            }
            else
                DataWindow.Open(pObjectArray);
        }

        private void CopyPaste(GenericMenu pMenu, Data pData)
        {
            pMenu.AddSeparator(string.Empty);
            pMenu.AddItem(new GUIContent("Copy"), false, () => _ClipBoardedData = pData);

            if (_ClipBoardedData != null)
            {
                pMenu.AddItem(new GUIContent("Paste/Data Values"), false, () =>
                {
                    string lName = pData.name;
                    EditorUtility.CopySerialized(_ClipBoardedData, pData);
                    pData.name = lName;
                });

                pMenu.AddItem(new GUIContent("Paste/As New Tab"), false, () =>
                {
                    Data lData = CreateInstance<Data>();
                    EditorUtility.CopySerialized(_ClipBoardedData, lData);
                    AssetDatabase.CreateAsset(lData, AssetDatabase.GenerateUniqueAssetPath($"{Parameter.GetCurrentBagPath()}/{pData.name}.asset"));
                    UpdateData();
                });
            }
            else
            {
                pMenu.AddDisabledItem(new GUIContent("Paste/Data Values"));
                pMenu.AddDisabledItem(new GUIContent("Paste/As New Tab"));
            }
        }

        private static void SetFolder()
        {
            string[] lPathArray = GetSubPaths(DATA_PATH).ToArray();
            int lInt = lPathArray.Length;

            string lParentPath = lPathArray[0];
            string lChildPath;


            for (int i = 1; i < lInt; i++)
            {
                lChildPath = lPathArray[i];
                if (!AssetDatabase.IsValidFolder(lChildPath)) AssetDatabase.CreateFolder(lParentPath, Path.GetFileName(lChildPath));
                lParentPath = lPathArray[i];
            }

            Parameter = GetHistoryParameter();
            ParameterPack = LastInstance.position.size.x > LastInstance.position.size.y ? Parameter.HorizontalParameterPack : Parameter.VerticalParameterPack;

            if (Parameter == null)
            {
                HistoryParameter lParameter = CreateInstance<HistoryParameter>();
                AssetDatabase.CreateAsset(lParameter, HISTORY_PARAMETER_PATH);
                Parameter = GetHistoryParameter();
                ParameterPack = LastInstance.position.size.x > LastInstance.position.size.y ? Parameter.HorizontalParameterPack : Parameter.VerticalParameterPack;
                if (!GitignoreHandlerWindow.IsGitignoreUpdated()) GitignoreHandlerWindow.Open();
            }

            if (Directory.EnumerateDirectories(Parameter.savePath).Count() <= 0)
            {
                AssetDatabase.CreateFolder(Parameter.savePath, DEFAULT_CATEGORY);
                AssetDatabase.CreateFolder(Parameter.savePath + "/" + DEFAULT_CATEGORY, DEFAULT_TAB);
                Parameter.SetCurrentBagPath(Parameter.savePath + "/" + DEFAULT_CATEGORY + "/" + DEFAULT_TAB);
            }

            AssetDatabase.Refresh();
        }

        private void SetTitleContent(string pCurrentTab)
        {
            titleContent = new GUIContent(Path.GetFileName(Directory.GetParent(pCurrentTab).ToString()) + "/" + Path.GetFileName(pCurrentTab), GetIconContent(TAB_ICON));
        }

        public static void UpdateAndRepaint()
        {
            LastInstance.UpdateLoadableGUIObject();
            LastInstance.m_FolderSelectable.ClearSelectedList();
            LastInstance.Repaint();
        }

        private void CheckEvent()
        {
            if (Event.current.isKey)
            {
                Event lEvent = Event.current;

                if (m_FolderSelectable.IsSelecting && (lEvent.keyCode == KeyCode.Delete || lEvent.keyCode == KeyCode.Backspace))
                {
                    CurrentData.TryRemoves(m_FolderSelectable.SelectList);
                    m_FolderSelectable.ClearSelectedList();
                    UpdateLoadableGUIObject();
                }
                else if (lEvent.control && lEvent.keyCode == KeyCode.A)
                {
                    for (int i = 0; i < _GuiContentArray.Length; i++)
                    {
                        m_FolderSelectable.ForceAddByControl(m_LoadableObjectArray[i].Guid);
                    }
                }
            }
        }

        private void CheckSize()
        {
            if (position.size.x > position.size.y)
            {
                _BeginHV = () => GUILayout.BeginHorizontal();
                _EndHV = () => GUILayout.EndHorizontal();

                GetButtonGUIRect = GetButtonHorizontalRect;
                GetTabGUIRect = GetTabHorizontalRect;
                _IsVertical = false;

                ParameterPack = Parameter.HorizontalParameterPack;
            }
            else
            {
                _BeginHV = () => GUILayout.BeginVertical();
                _EndHV = () => GUILayout.EndVertical();

                GetButtonGUIRect = GetButtonVerticalRect;
                GetTabGUIRect = GetTabVerticalRect;

                TabVerticalSize = LastSize.x * .2f;
                ButtonVerticalSize = LastSize.x * .8f;
                _IsVertical = true;
                Debug.Log(Parameter == null);
                Debug.Log(Parameter.VerticalParameterPack == null);
                ParameterPack = Parameter.VerticalParameterPack;
            }
            LastSize = position.size;
        }

        private void OnFocus() => LastInstance = this;
        private void OnDestroy()
        {
            if (LastInstance == this)
            {
                LastInstance = null;
                while (LastInstance == null && _TabBagList.Count > 0)
                {
                    LastInstance = _TabBagList[0];
                    if (LastInstance == null) _TabBagList.RemoveAt(0);
                }
            }
        }

        #region Shortcut

        [Shortcut("TSearch", KeyCode.T, ShortcutModifiers.Shift | ShortcutModifiers.Control)]
        private static void OpenSearchWindow()
        {
            SearchWindow lWindow = SearchWindow.Open(TabBagWindow.LastInstance.CurrentData, true, false, false);
            lWindow.onObjectSelected += (pObjectArray) => TabBagWindow.LastInstance.CurrentData.TryAdds(pObjectArray);
        }

        [Shortcut("New Bag", KeyCode.N, ShortcutModifiers.Shift)]
        public static void ShortCutBagMakerWindow() => BagMakerWindow.Open();


        [Shortcut("Test1", KeyCode.Alpha1, ShortcutModifiers.Shift)]
        public static void Test1()
        {
            if (TabBagWindow.LastInstance.DataList.Count - 1 >= 0)
            {
                TabBagWindow.LastInstance.CurrentData = TabBagWindow.LastInstance.DataList[0];
                UpdateAndRepaint();
            }
        }

        [Shortcut("Test2", KeyCode.Alpha2, ShortcutModifiers.Shift)]
        public static void Test2()
        {
            if (TabBagWindow.LastInstance.DataList.Count - 1 >= 1)
            {
                TabBagWindow.LastInstance.CurrentData = TabBagWindow.LastInstance.DataList[1];
                UpdateAndRepaint();
            }
        }

        [Shortcut("Test3", KeyCode.Alpha3, ShortcutModifiers.Shift)]
        public static void Test3()
        {
            if (TabBagWindow.LastInstance.DataList.Count - 1 >= 2)
            {
                TabBagWindow.LastInstance.CurrentData = TabBagWindow.LastInstance.DataList[2];
                UpdateAndRepaint();
            }
        }

        [Shortcut("Test4", KeyCode.Alpha4, ShortcutModifiers.Shift)]
        public static void Test4()
        {
            if (TabBagWindow.LastInstance.DataList.Count - 1 >= 3)
            {
                TabBagWindow.LastInstance.CurrentData = TabBagWindow.LastInstance.DataList[3];
                UpdateAndRepaint();
            }
        }

        [Shortcut("Test5", KeyCode.Alpha5, ShortcutModifiers.Shift)]
        public static void Test5()
        {
            if (TabBagWindow.LastInstance.DataList.Count - 1 >= 4)
            {
                TabBagWindow.LastInstance.CurrentData = TabBagWindow.LastInstance.DataList[4];
                UpdateAndRepaint();
            }
        }

        [Shortcut("Test6", KeyCode.Alpha6, ShortcutModifiers.Shift)]
        public static void Test6()
        {
            if (TabBagWindow.LastInstance.DataList.Count - 1 >= 5)
            {
                TabBagWindow.LastInstance.CurrentData = TabBagWindow.LastInstance.DataList[5];
                UpdateAndRepaint();
            }
        }

        [Shortcut("Test7", KeyCode.Alpha7, ShortcutModifiers.Shift)]
        public static void Test7()
        {
            if (TabBagWindow.LastInstance.DataList.Count - 1 >= 6)
            {
                TabBagWindow.LastInstance.CurrentData = TabBagWindow.LastInstance.DataList[6];
                UpdateAndRepaint();
            }
        }

        [Shortcut("Test8", KeyCode.Alpha8, ShortcutModifiers.Shift)]
        public static void Test8()
        {
            if (TabBagWindow.LastInstance.DataList.Count - 1 >= 7)
            {
                TabBagWindow.LastInstance.CurrentData = TabBagWindow.LastInstance.DataList[7];
                UpdateAndRepaint();
            }
        }

        [Shortcut("Test9", KeyCode.Alpha9, ShortcutModifiers.Shift)]
        public static void Test9()
        {
            if (TabBagWindow.LastInstance.DataList.Count - 1 >= 8)
            {
                TabBagWindow.LastInstance.CurrentData = TabBagWindow.LastInstance.DataList[8];
                UpdateAndRepaint();
            }
        }

        [Shortcut("Test10", KeyCode.Alpha0, ShortcutModifiers.Shift)]
        public static void Test10()
        {
            if (TabBagWindow.LastInstance.DataList.Count - 1 >= 9)
            {
                TabBagWindow.LastInstance.CurrentData = TabBagWindow.LastInstance.DataList[9];
                UpdateAndRepaint();
            }
        }

        [Shortcut("Go Righttt", KeyCode.RightArrow, ShortcutModifiers.Control | ShortcutModifiers.Shift)]
        public static void GoRighttt()
        {
            int lIndex = TabBagWindow.LastInstance.DataList.IndexOf(TabBagWindow.LastInstance.CurrentData);
            TabBagWindow.LastInstance.CurrentData = TabBagWindow.LastInstance.DataList[lIndex < TabBagWindow.LastInstance.DataList.Count - 1 ? lIndex + 1 : 0];
            UpdateAndRepaint();
        }

        [Shortcut("Go Leftttt", KeyCode.LeftArrow, ShortcutModifiers.Control | ShortcutModifiers.Shift)]
        public static void GoLefttt()
        {
            int lIndex = TabBagWindow.LastInstance.DataList.IndexOf(TabBagWindow.LastInstance.CurrentData);
            TabBagWindow.LastInstance.CurrentData = TabBagWindow.LastInstance.DataList[lIndex > 0 ? lIndex - 1 : TabBagWindow.LastInstance.DataList.Count - 1];
            UpdateAndRepaint();
        }

        #endregion
    }
}