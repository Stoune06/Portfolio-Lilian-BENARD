using System.Collections;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Enum;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Other
{
    
    public static class RaritySettings
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        private static readonly Dictionary<ERarity, Color> Colors = new Dictionary<ERarity, Color> 
        {
            { ERarity.COMMON, Color.blue },
            { ERarity.RARE, new Color(1f, .5f, 0f) },
            { ERarity.EPIC, new Color(.3f, 0, .75f) },
            { ERarity.LEGENDARY, new Color(.5f, 0f, .2f, 1f) },
        };

        public static Color GetColor(ERarity pRarity) => Colors.TryGetValue(pRarity, out Color lColor) ? lColor : Color.white;
        
    }
}