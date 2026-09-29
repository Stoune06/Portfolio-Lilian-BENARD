using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Other;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.SO
{
    [CreateAssetMenu(fileName = Utils.ENEMY_FILE_NAME, menuName = Utils.ENEMY_MENU_NAME)]
    public class EnemyDataSO : ScriptableObject
    {
        public EType type;
        public GameObject prefab;
    }
}