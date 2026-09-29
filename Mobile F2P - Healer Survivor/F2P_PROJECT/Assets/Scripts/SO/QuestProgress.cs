using System;
using System.Collections;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Data;
using Newtonsoft.Json;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Data
{
    
    [Serializable]
    public class QuestProgress
    {
        public string questID;
        public int currentAmount;
        public bool isRedeemed;
        
        [JsonIgnore] public QuestData data;
        [JsonIgnore] public bool IsComplete => data != null && currentAmount >= data.goalAmount;
    }
}