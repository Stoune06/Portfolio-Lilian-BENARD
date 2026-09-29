using Com.IsartDigital.HealerSurvivor.Other;
using Com.IsartDigital.HealerSurvivor.SO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Tools
{
    [CustomEditor(typeof(EnemyDataSO))]
    public class EnemyDataEditor : Editor
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        private const int SPACE_SIZE = 20;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        public override void OnInspectorGUI()
        {
            EnemyDataSO lMySO = (EnemyDataSO)target;
            GUILayout.TextArea(Utils.SO_DESCRIPTION_ENEMY_DATA);
            GUILayout.Space(SPACE_SIZE);

            DrawDefaultInspector();
        }
    }
}