#if UNITY_EDITOR
using Tooling.Attributes;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

//Author : MERFOUD Kelyan

namespace Tooling.Editors
{
    internal class AutoLoadWindow : EditorWindow
    {
        private Texture2D _BinIcon;

        private AutoScriptable _AutoScriptable;
        private SerializedObject _SerializedScriptable;

        private SerializedProperty _SerializedLoadableArray;


        [MenuItem("GameObject/AutoLoad")]
        public static AutoLoadWindow Open()
        {
            AutoLoadWindow lWindow = GetWindow<AutoLoadWindow>();

            lWindow.titleContent = new GUIContent("AutoLoad", (Texture2D)EditorGUIUtility.IconContent("d_InputField Icon").image);
            lWindow._BinIcon = (Texture2D)EditorGUIUtility.IconContent("d_TreeEditor.Trash").image;

            EditorApplication.playModeStateChanged += lWindow.OnPlay;
            return lWindow;
        }

        private void CreateGUI()
        {
            SetScriptableArray();
        }

        private void OnGUI()
        {
            HandleDragAndDrop();
            ShowCategories();
            ShowAutoLoadedPrefabs();
            Repaint();
            _SerializedScriptable.ApplyModifiedProperties();
        }

        private void ShowCategories()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Name", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.Label("Path", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.Label("Global Variable", EditorStyles.boldLabel);

            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
        }

        private void ShowAutoLoadedPrefabs()
        {
            LoadableObject lLoadable;
            SerializedProperty lSerializedEnumFlag;

            if (_AutoScriptable.LoadableArray != null && _AutoScriptable.LoadableArray.Length != 0)
            {
                for (int i = _AutoScriptable.LoadableArray.Length - 1; i >= 0; i--)
                {
                    lLoadable = _AutoScriptable.LoadableArray[i];

                    lSerializedEnumFlag = _SerializedLoadableArray.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(LoadableObject.loadType));
                    if (lLoadable.gameObject == null)
                    {
                        _SerializedLoadableArray.DeleteArrayElementAtIndex(i);
                        Debug.LogError($"The element \"{i}\" was deleted from Autoload Window because it was null");
                        continue;
                    }

                    GUILayout.BeginHorizontal();

                    GUILayout.Label(lLoadable.gameObject.name);
                    GUILayout.FlexibleSpace();

                    GUILayout.Label(AssetDatabase.GetAssetPath(lLoadable.gameObject));
                    GUILayout.FlexibleSpace();


                    HandleEnumFlagAsButtons(lSerializedEnumFlag, lLoadable.gameObject is SceneHolder);

                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button(_BinIcon)) _SerializedLoadableArray.DeleteArrayElementAtIndex(i);
                    GUI.backgroundColor = Color.white;

                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUIStyle lGUIStyle = new GUIStyle(EditorStyles.boldLabel);
                lGUIStyle.fontSize = 25;
                GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();
                GUILayout.Label("Start by adding Object with Drag and Drop", lGUIStyle);
            }
        }


        private void HandleEnumFlagAsButtons(SerializedProperty pLoadTypeFlag, bool pIsScene)
        {
            bool lStartDestroy = (pLoadTypeFlag.enumValueFlag & (int)LoadType.DestroyOnLoad) != 0;
            bool lStartActive = (pLoadTypeFlag.enumValueFlag & (int)LoadType.Active) != 0;

            bool lGUIDestroy = lStartDestroy;

            if (!pIsScene) 
                lGUIDestroy = GUILayout.Toggle(lStartDestroy, "Destroy on Load"); 

            bool lGUIActive = GUILayout.Toggle(lStartActive, "Is Active");

            if (lGUIActive != lStartActive || lGUIDestroy != lStartDestroy)
            {
                pLoadTypeFlag.enumValueFlag = (lGUIActive ? (int)LoadType.Active : 0) + (lGUIDestroy ? (int)LoadType.DestroyOnLoad : 0);
            }
        }

        private void HandleNew(UnityObject[] pObject)
        {
            UnityObject lObject = default;
            foreach (UnityObject item in pObject)
            {
                if (item != null)
                {
                    _SerializedLoadableArray.arraySize += 1;
                    SerializedProperty lNewProperty = _SerializedLoadableArray.GetArrayElementAtIndex(_SerializedLoadableArray.arraySize - 1);
                    lNewProperty.FindPropertyRelative(nameof(LoadableObject.loadType)).intValue = (int)LoadType.Active;
                    lObject = item;

                    if (item is SceneAsset lSceneAsset)
                    {
                        SceneHolder lSceneHolder = new SceneHolder(AssetDatabase.GetAssetPath(item));
                        lSceneHolder.sceneName = System.IO.Path.GetFileNameWithoutExtension(lSceneHolder.path);
                        AssetDatabase.CreateAsset(lSceneHolder, AssetDatabase.GenerateUniqueAssetPath(AutoScriptable.GetNewPathWithName(lSceneHolder.sceneName)));
                        AssetDatabase.Refresh();
                        lObject = lSceneHolder;
                    }

                    lNewProperty.FindPropertyRelative(nameof(LoadableObject.gameObject)).objectReferenceValue = lObject;
                    lObject = null;
                }
            }
        }

        private void SetScriptableArray()
        {
            _AutoScriptable = AssetDatabase.LoadAssetAtPath<AutoScriptable>(AutoScriptable.PATH);

            if (_AutoScriptable == null)
            {
                if (!AssetDatabase.IsValidFolder(AutoScriptable.RESOURCE_PATH)) AssetDatabase.CreateFolder(AutoScriptable.ASSET_PATH, AutoScriptable.RESOURCE);
                if (!AssetDatabase.IsValidFolder(AutoScriptable.TOOLING_PATH)) AssetDatabase.CreateFolder(AutoScriptable.RESOURCE_PATH, AutoScriptable.TOOLING);
                if (!AssetDatabase.IsValidFolder(AutoScriptable.AUTOLOAD_PATH)) AssetDatabase.CreateFolder(AutoScriptable.TOOLING_PATH, AutoScriptable.AUTOLOAD);

                AssetDatabase.CreateAsset(new AutoScriptable(), AutoScriptable.PATH);
                AssetDatabase.Refresh();
                _AutoScriptable = AssetDatabase.LoadAssetAtPath<AutoScriptable>(AutoScriptable.PATH);
            }

            _SerializedScriptable = new SerializedObject(_AutoScriptable);

            _SerializedLoadableArray = _SerializedScriptable.FindProperty(nameof(AutoScriptable.LoadableArray));
        }

        private void KillUnusedSceneHolder()
        {
            string[] lPathArray = AssetDatabase.FindAssets($"t:{nameof(SceneHolder)}", new[] { AutoScriptable.TOOLING_PATH });
            System.Collections.Generic.List<string> lPathList = new System.Collections.Generic.List<string>();

            foreach (LoadableObject lLoadable in _AutoScriptable.LoadableArray)
            {
                if (lLoadable.gameObject is SceneHolder lSceneHolder) lPathList.Add(lSceneHolder.path);
                else lPathList.Add(AssetDatabase.GetAssetPath(lLoadable.gameObject));
            }

            SceneHolder lHolder;
            string lPath;

            foreach (string pGUID in lPathArray)
            {
                lPath = AssetDatabase.GUIDToAssetPath(pGUID);
                lHolder = AssetDatabase.LoadAssetAtPath<SceneHolder>(lPath);
                if (lHolder != null && !lPathList.Contains(lHolder.path))
                {
                    AssetDatabase.DeleteAsset(lPath);
                }
            }
            AssetDatabase.Refresh();
        }

        private void HandleDragAndDrop()
        {
            Event lEvent = Event.current;
            Rect lRect = new Rect(0f, 0f, position.width, position.height);

            if (lRect.Contains(lEvent.mousePosition))
            {
                switch (lEvent.type)
                {
                    case EventType.DragUpdated:
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        lEvent.Use();
                        break;

                    case EventType.DragPerform:
                        DragAndDrop.AcceptDrag();
                        HandleNew(DragAndDrop.objectReferences);
                        lEvent.Use();
                        break;
                }
            }
        }

        private void OnPlay(PlayModeStateChange pPlayMode)
        {
            if (pPlayMode == PlayModeStateChange.ExitingEditMode) Close();
        }

        private void OnDestroy() 
        {
            EditorApplication.playModeStateChanged -= OnPlay;
            KillUnusedSceneHolder();
        } 
    }
}
#endif