using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.SO;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Data
{
    [System.Serializable]
    public class ShopData
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        public List<string> cardsNamesInShop;

        public ShopData(List<string> pCardsNamesInShop)
        {
           cardsNamesInShop = pCardsNamesInShop;
        }
    }
}