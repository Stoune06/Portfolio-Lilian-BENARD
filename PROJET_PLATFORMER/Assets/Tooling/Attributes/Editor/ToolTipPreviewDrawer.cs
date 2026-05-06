using Tooling;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TooltipPreview))]
public class ToolTipPreviewDrawer : PropertyDrawer
{
    public override void OnGUI(Rect pPosition, SerializedProperty pProperty, GUIContent pLabel)
    {
        TooltipPreview lPreview = (TooltipPreview)attribute;

        if (pPosition.Contains(Event.current.mousePosition) && pProperty.objectReferenceValue != null)
        {
            Object lObject = pProperty.objectReferenceValue;

            Vector2 lMousePosition = Event.current.mousePosition;

            Texture2D lTexture = AssetPreview.GetAssetPreview(lObject);

            if (lTexture != null)
                EditorGUI.DrawPreviewTexture(new Rect(lMousePosition.x, lMousePosition.y - lPreview.size, lPreview.size, lPreview.size), lTexture);
            HandleUtility.Repaint();
        }

        EditorGUI.PropertyField(pPosition, pProperty, pLabel);
    }
}