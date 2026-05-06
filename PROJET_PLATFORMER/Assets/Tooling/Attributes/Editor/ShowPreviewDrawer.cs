using Tooling;
using UnityEditor;
using UnityEngine;

namespace Tooling.Attributes.Editors
{
    [CustomPropertyDrawer(typeof(ShowPreview))]
    public class ShowPreviewDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pPosition, SerializedProperty pProperty, GUIContent pLabel)
        {
            ShowPreview lPreview = (ShowPreview)attribute;

            Rect lFieldRect = new Rect(pPosition.x, pPosition.y, pPosition.width - lPreview.size - 5, pPosition.height);
            Rect lPreviewRect = new Rect(pPosition.x + pPosition.width - lPreview.size, pPosition.y, lPreview.size, lPreview.size);

            EditorGUI.PropertyField(lFieldRect, pProperty, pLabel);

            if (pProperty.objectReferenceValue != null)
            {
                Texture2D pPreview = AssetPreview.GetAssetPreview(pProperty.objectReferenceValue);
                if (pPreview == null) pPreview = AssetPreview.GetMiniThumbnail(pProperty.objectReferenceValue);
                if (pPreview != null)
                {
                    GUI.DrawTexture(lPreviewRect, pPreview, ScaleMode.ScaleToFit);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => Mathf.Max(EditorGUIUtility.singleLineHeight, ((ShowPreview)attribute).size);
    }
}