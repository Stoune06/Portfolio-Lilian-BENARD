using Com.IsartDigital.HealerSurvivor.Other;
using Com.IsartDigital.HealerSurvivor.SO;
using UnityEditor;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Tools
{
    [CustomEditor(typeof(EnemyWaveSO))]
    public class EnemyWaveEditor : Editor
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        private const int RECT_SIZE = 300;
        private const int SPACE_SIZE = 20;

        public override void OnInspectorGUI()
        {
            EnemyWaveSO lMySO = (EnemyWaveSO)target;
            GUILayout.TextArea(Utils.SO_DESCRIPTION_ENEMY);
            GUILayout.Space(SPACE_SIZE);

            DrawDefaultInspector();

            if (lMySO.WaveIllustration != null)
            {
                GUILayout.Space(SPACE_SIZE);
                GUILayout.Label(Utils.ILLUSTRATION_SO_CIRCLE, EditorStyles.boldLabel);
                GUILayout.Space(SPACE_SIZE);
                Rect lRect = GUILayoutUtility.GetRect(RECT_SIZE, RECT_SIZE);
                GUI.DrawTexture(lRect, lMySO.WaveIllustration.texture, ScaleMode.ScaleToFit);
            }
        }
    }
}