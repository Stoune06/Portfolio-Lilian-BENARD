using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Editors.History.Utils;

namespace Editors.History
{
    public class DataWindow : FocusableWindow
    {
        private Data _Data;
        private SerializedObject _SerializedData;
        List<SerializedProperty> _DataPropertyList = new List<SerializedProperty>();

        System.Type _Type;

        public Object[] temporalData;
        private string _CurrentPath;

        public static DataWindow Open() => SetWindowAndOpen();
        public static DataWindow Open(Object[] pObjectArray)
        {
            DataWindow lWindow = SetWindowAndOpen();
            lWindow.SetType(pObjectArray);
            return lWindow;
        }

        private static DataWindow SetWindowAndOpen()
        {
            DataWindow lWindow = GetWindow<DataWindow>();
            lWindow.titleContent = new GUIContent("Tab Creator", (Texture2D)EditorGUIUtility.IconContent(FOLDER_ICON).image);
            return lWindow;
        }

        private void CreateGUI()
        {
            _Data = ScriptableObject.CreateInstance<Data>();
            SetSeriazableVariable();
        }

        private void OnGUI()
        {
            ShowParameter();
            FocusControl();
            _CurrentPath = GetDataFullPath();

            ListenToType();
            Create();
            ListenEnter();
            _SerializedData.ApplyModifiedProperties(); //TODO OPTIMIZE
        }

        private void SetSeriazableVariable()
        {
            _SerializedData = new SerializedObject(_Data);
            SerializedProperty pSerialized = _SerializedData.GetIterator();
            _DataPropertyList.Clear();

            while (pSerialized.NextVisible(true))
            {
                if(pSerialized.name == nameof(Data.tabName))
                {
                    _DataPropertyList.Add(pSerialized.Copy());
                    pSerialized.stringValue = _Data.TypeName;
                    continue;
                }
                if (pSerialized.name != "<SelectColor>k__BackingField" && pSerialized.name != nameof(_Data.GUIDList))
                {
                    _DataPropertyList.Add(pSerialized.Copy());
                }
            }
        }

        private void ShowParameter()
        {
            GUI.SetNextControlName(m_GuiName);
            foreach (SerializedProperty item in _DataPropertyList)
            {
                EditorGUILayout.PropertyField(item);
                GUILayout.Space(3.5f);
            }
        }

        protected void ListenEnter()
        {
            if (Event.current.keyCode == KeyCode.Return)
            {
                CreateData();
                Event.current.Use();
            }
        }

        private void ListenToType()
        {
            GUILayout.Space(10f);

            GUILayout.Label(_Data.TypeName == string.Empty ? $"Please drop an object with the wished type into the window"
                : $"This Bag will store object of the {_Data.TypeName} type \n(You can press Enter to validate)");
        }

        private void Create() //TODO CHECK IF FILE PATH VALID
        {
            bool pPlaceable = IsDataPlaceable();
            GUI.backgroundColor = pPlaceable ? Color.green : Color.red;

            if (GUILayout.Button("Create") && pPlaceable)
            {
                CreateData();
            }
        }

        private void CreateData()
        {
            if (IsDataPlaceable())
            {
                Data lData = ScriptableObject.CreateInstance<Data>();

                lData.tabName = _Data.tabName;
                lData.logo = _Data.logo;
                lData.typeCompleteName = _Data.typeCompleteName;
                lData.tabName = _Data.tabName;
                lData.GUIDList = _Data.GUIDList.ToList();

                OnValidate();

                AssetDatabase.CreateAsset(lData, _CurrentPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                TabBagWindow.LastInstance.UpdateData();

                EditorApplication.delayCall += () =>
                {
                    lData = AssetDatabase.LoadAssetAtPath<Data>(_CurrentPath);
                    TabBagWindow.LastInstance.UpdateData();
                    TabBagWindow.LastInstance.CurrentData = lData;
                    TabBagWindow.UpdateAndRepaint();
                    Close();
                };
            }
        }


        private string GetDataFullPath() => TabBagWindow.Parameter.GetCurrentBagPath() + "/" + _Data.tabName + ASSET_EXTENSION;

        public void SetType(Object[] pObjectArray)
        {
            Object lObject = pObjectArray[0];
            _Type = lObject.GetType();
            _Data.TypeName = _Type.Name;

            _Data.logo = (Texture2D)AssetDatabase.GetCachedIcon(AssetDatabase.GetAssetPath(lObject));

            foreach (var item in pObjectArray)
            {
                _Data.GUIDList.Add(ObjectToGUID(item));
            }

            SetSeriazableVariable();
        }

        private void OnValidate()
        {
            _SerializedData.ApplyModifiedProperties();
        }

        public bool IsDataPlaceable()
            => !File.Exists(_CurrentPath) &&
            Directory.GetParent(_CurrentPath).Exists &&
            _Data.TypeName != string.Empty;
    }
}