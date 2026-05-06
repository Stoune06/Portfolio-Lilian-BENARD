using Com.IsartDigital.HealerSurvivor.Other;
using Com.IsartDigital.HealerSurvivor.SO;
using UnityEditor;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Tools
{
    [CustomEditor(typeof(WaveBuilderSO))]
    public class WaveBuilderEditor : Editor
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        private const int SPACE_SIZE = 20;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        public override void OnInspectorGUI()
        {
            WaveBuilderSO lMySO = (WaveBuilderSO)target;
            GUILayout.TextArea(Utils.SO_DESCRIPTION_WAVE_BUILDER);
            GUILayout.Space(SPACE_SIZE);

            DrawDefaultInspector();
        }
    }
}