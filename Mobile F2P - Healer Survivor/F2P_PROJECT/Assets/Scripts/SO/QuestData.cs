using System.Collections;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Other;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Data
{
    [CreateAssetMenu(fileName = Utils.QUEST_FILE_NAME, menuName = Utils.QUEST_MENU_NAME)]
    public class QuestData : ScriptableObject
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        public string questID;
        public string questName;
        public string description;
        public int goalAmount;
        public int rewardAmount;
        public EQuestType questType;
        public ECurrencyType rewardType;
    }
}