using UnityEditor;
using UnityEngine;
using static Editors.History.Utils;

namespace Editors.History
{
    internal class LoadableGUIObject
    {
        public string Path { get; private set; }
        public string Guid { get; private set; }
        public string Name { get; private set; }
        public string ChoppedName { get; private set; }
        public Object TargetObject { get; private set; }
        public GUIContent Content { get; private set; }
        public Texture2D Texture { get; private set; }

        public LoadableGUIObject(string pGuid, ShowType pShowType)
        {
            Load(pGuid);
            UpdateTexture(pShowType);
            UpdateGUIContent(pShowType);
        }

        private void Load(string pGuid)
        {
            Guid = pGuid;
            Path = AssetDatabase.GUIDToAssetPath(Guid);
            TargetObject = AssetDatabase.LoadAssetAtPath<Object>(Path);

            UpdateName();
        }

        public void UpdateGUIContent(ShowType pShowType)
        {
            switch (pShowType)
            {
                case ShowType.None:
                    Texture = null;
                    Content = new GUIContent(ChoppedName, Name);
                    break;
                case ShowType.Thumbnail:
                    Texture = AssetPreview.GetMiniThumbnail(TargetObject);
                    Content = new GUIContent(ChoppedName, Texture, Name);
                    break;
                case ShowType.Preview:
                    Texture = AssetPreview.GetAssetPreview(TargetObject);
                    if (Texture == null) Texture = AssetPreview.GetMiniThumbnail(TargetObject);
                    Content = new GUIContent(ChoppedName, Texture, Name);
                    break;
                default:
                    break;
            }
        }

        public void UpdateTexture(ShowType pShowType)
        {
            switch (pShowType)
            {
                case ShowType.None:
                    Texture = null;
                    break;
                case ShowType.Thumbnail:
                    Texture = AssetPreview.GetMiniThumbnail(TargetObject);
                    break;
                case ShowType.Preview:
                    Texture = AssetPreview.GetAssetPreview(TargetObject);
                    if (Texture == null) Texture = AssetPreview.GetMiniThumbnail(TargetObject);
                    break;
                default:
                    break;
            }
        }

        public void UpdateName()
        {
            Name = System.IO.Path.GetFileNameWithoutExtension(Path);
            ChoppedName = GetChoppedName(Name);
        }

        public void UpdatePathFromGUID() => Path = AssetDatabase.GUIDToAssetPath(Guid);
    }
}