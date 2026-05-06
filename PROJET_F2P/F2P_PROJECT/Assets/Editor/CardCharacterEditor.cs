using Com.IsartDigital.HealerSurvivor.Other;
using Com.IsartDigital.HealerSurvivor.SO;
using UnityEditor;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Tools
{
    [CustomEditor(typeof(CardCharacterSO))]
    public class CardCharacterEditor : Editor
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        private const int SPACE_SIZE = 20;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        public override void OnInspectorGUI()
        {
            CardCharacterSO lMySO = (CardCharacterSO)target;
            GUILayout.TextArea(Utils.SO_DESCRIPTION_CARD);
            GUILayout.Space(SPACE_SIZE);

            DrawDefaultInspector();
        }
    }
}