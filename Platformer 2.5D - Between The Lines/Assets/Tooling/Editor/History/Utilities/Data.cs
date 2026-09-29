using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using static Editors.History.Utils;

namespace Editors.History
{
    internal class Data : ScriptableObject
    {
        public string tabName = "Default";
        public Texture2D logo;
        [HideInInspector] public string typeCompleteName = string.Empty;

        [HideInInspector] public string TypeName = string.Empty;

        [HideInInspector] public List<string> GUIDList = new List<string>();

        [HideInInspector] public string currentObject = string.Empty;

        public System.Action onValidate;

        public bool TryAdd(Object pObject, string pGUID)
        {
            if (GUIDList.Count + 1 < TabBagWindow.ParameterPack.maxHistoryNumber && !GUIDList.Contains(pGUID) && pGUID != string.Empty &&
                pObject.GetType().Name == TypeName)
            {
                //Undo.RecordObject(this, "added");
                //Undo.undoRedoEvent += (in UndoRedoInfo p) => { Debug.Log(p.); };
                GUIDList.Add(pGUID);
                EditorUtility.SetDirty(this);
                return true;
            }
            return false;
        }

        public bool TryAdd(Object pObject) => TryAdd(pObject, ObjectToGUID(pObject));
        public bool TryAdd(LoadableGUIObject pObject) => TryAdd(pObject.TargetObject, pObject.Guid);

        public void TryAdds(IEnumerable<LoadableGUIObject> pObjectEnumerable)
        {
            foreach (LoadableGUIObject item in pObjectEnumerable)
            {
                TryAdd(item.TargetObject);
            }
        }

        public void TryRemove(string pGUID)
        {
            GUIDList.Remove(pGUID);
            EditorUtility.SetDirty(this);
        }

        public void TryRemoves(IEnumerable<string> pGUIDEnumerable)
        {
            foreach (string item in pGUIDEnumerable)
            {
                TryRemove(item);
            }
        }

        private void OnValidate()
        {
            onValidate?.Invoke();
        }

        private void OnDestroy()
        {
            onValidate = null;
        }

        internal void RemoveAllPath() => GUIDList.Clear();

        public override string ToString()
        {
            StringBuilder lStringBuilder = new StringBuilder();
            foreach (string item in GUIDList)
            {
                lStringBuilder.Append("[");
                lStringBuilder.Append(item);
                lStringBuilder.Append("] ");
            }
            return lStringBuilder.ToString();
        }
    }

    public enum ShowType { None, Thumbnail, Preview }
    public enum ToolTipType { None, Name, Path, Preview }
}