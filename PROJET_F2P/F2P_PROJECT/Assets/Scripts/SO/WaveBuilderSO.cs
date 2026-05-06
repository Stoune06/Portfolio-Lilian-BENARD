using Com.IsartDigital.HealerSurvivor.Other;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.SO
{
    [CreateAssetMenu(fileName = Utils.WAVE_FILE_NAME, menuName = Utils.WAVE_BUILDER_MENU_NAME)]
    public class WaveBuilderSO : ScriptableObject
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [Header(Utils.HEADER_WAVE_BUILDER)]
        [SerializeField] private List<EnemyWaveSO> _EnemyToSpawn;

        public List<EnemyWaveSO> EnemyToSpawn => _EnemyToSpawn;
    }
}