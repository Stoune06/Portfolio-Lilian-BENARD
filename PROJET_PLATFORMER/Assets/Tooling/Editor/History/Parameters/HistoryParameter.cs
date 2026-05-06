using static Editors.History.Utils;
using UnityEditor;
using UnityEngine;
using System;

namespace Editors.History.Parameters
{
    internal class HistoryParameter : ScriptableObject
    {
        [SerializeField] internal string savePath = DATA_PATH;
        [SerializeField] internal Data defaultParameterData;

        [SerializeField] private string _CurrentBagPath = string.Empty;

        [SerializeField, HideInInspector] private int _CurrentTabNumber = 0;
        public int CurrentTabNumber { get => _CurrentTabNumber; set => _CurrentTabNumber = value < 0 ? 0 : value; }

        [SerializeField] internal string path = HISTORY_PARAMETER_PATH;

        [SerializeField] internal ParameterPack HorizontalParameterPack;
        [SerializeField] internal ParameterPack VerticalParameterPack;

        public string GetCurrentBagPath()
        {
            return _CurrentBagPath;
        }
        public void SetCurrentBagPath(string pPath)
        {
            _CurrentBagPath = pPath;
            EditorUtility.SetDirty(this);
        }

        public string GetPath() => AssetDatabase.GetAssetPath(this);
    }

    [Serializable]
    internal class ParameterPack
    {
        public ShowType showType = ShowType.Preview;
        public ToolTipType toolTipType = ToolTipType.Name;
        [Range(3, 1024)] public int maxHistoryNumber = 30;
    }
}