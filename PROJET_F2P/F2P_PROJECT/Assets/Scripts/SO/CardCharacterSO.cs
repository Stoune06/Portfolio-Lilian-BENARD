using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Other;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.SO
{
    [CreateAssetMenu(fileName = Utils.CARD_FILE_NAME, menuName = Utils.CARD_CHARACTER_MENU_NAME)]
    public class CardCharacterSO : ScriptableObject
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private string _CardName;
        [SerializeField] private Sprite _CardIcon;
        [SerializeField] private Sprite _CardDescription;
        [SerializeField] private GameObject _PrefabCharacter; 
        [SerializeField] private float _Health;
        [SerializeField] private float _Damage;
        [SerializeField] private ERarity _Rarity;
        [SerializeField] private int _Price;

        public string CardName => _CardName;
        public Sprite CardIcon => _CardIcon;
        public Sprite CardDescription => _CardDescription;
        public int Price => _Price;
        public float Health => _Health;
        public float Damage => _Damage;
        public ERarity Rarity => _Rarity;
        public GameObject PrefabCharacter => _PrefabCharacter;
    }
}