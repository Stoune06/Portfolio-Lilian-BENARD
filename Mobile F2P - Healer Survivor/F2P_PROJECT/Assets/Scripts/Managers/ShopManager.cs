using Com.IsartDigital.HealerSurvivor.Other;
using Com.IsartDigital.HealerSurvivor.Data;
using Com.IsartDigital.HealerSurvivor.SO;
using Com.IsartDigital.HealerSurvivor.UI;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Manager
{
    
    public class ShopManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private TextMeshProUGUI _TimeText;
        [SerializeField] private Transform _ShopContainer;
        [SerializeField] private GameObject _ShopCardPrefab;
        [SerializeField] private List<Transform> _ShopSlots;
        
        private readonly List<CardCharacterSO> _AvailableItems = new List<CardCharacterSO>();
        private const int SHOP_SIZE = 11;
        
        private TimeManager _TimeManager => TimeManager.Instance;
        private GameManager _GameManager => GameManager.Instance;
        private DeckBuilderManager _DeckBuilderManager => DeckBuilderManager.Instance;
        
        private string _ShopDataPath => Path.Combine(Application.persistentDataPath, Utils.SHOP_JSON_FILE_NAME);
        
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            GameManager.Instance.onMidNight += RefreshShop;
            
            if (File.Exists(_ShopDataPath)) 
                LoadShop();
            else 
                RefreshShop();
        }
        
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update() => UpdateTime();

        private void RefreshShop()
        {
            ClearShopUI();
            _AvailableItems.Clear();
            List<CardCharacterSO> lAllCards = _DeckBuilderManager.allCards;
            List<CardCharacterSO> lPotentialCards = new List<CardCharacterSO>(lAllCards);
            
            for (int i = 0; i < SHOP_SIZE && lPotentialCards.Count > 0; i++)
            {
                int lIndex = Random.Range(0, lPotentialCards.Count);
                _AvailableItems.Add(lPotentialCards[lIndex]);
                lPotentialCards.RemoveAt(lIndex);
            }

            DisplayShop();
            SaveCurrentShop();
        }

        private void DisplayShop()
        {
            for (int i = 0; i < _AvailableItems.Count; i++) 
            {
                if (i >= _ShopSlots.Count) 
                    break;

                GameObject lGo = Instantiate(_ShopCardPrefab, _ShopSlots[i]);
        
                RectTransform lRT = lGo.GetComponent<RectTransform>();
                lRT.anchoredPosition = Vector2.zero;
                lRT.sizeDelta = Vector2.zero;
                lRT.anchorMin = Vector2.zero;
                lRT.anchorMax = Vector2.one;

                if (lGo.TryGetComponent(out ShopCardDisplay lDisplay))
                    lDisplay.Init(_AvailableItems[i]);
            }
        }
        
        private void UpdateTime() => _TimeText.text = Utils.DAILY_REWARD_TIME_TXT + _TimeManager.TimeUntilMidnightString;
        
        private void SaveCurrentShop()
        {
            List<string> lNames = new List<string>();
            
            foreach (CardCharacterSO lCard in _AvailableItems)
                lNames.Add(lCard.CardName);

            ShopData lData = new ShopData(lNames);
    
            string lValueJson = JsonConvert.SerializeObject(lData, Formatting.Indented);
            File.WriteAllText(_ShopDataPath, lValueJson);
        }
        
        private void LoadShop()
        {
            if (!File.Exists(_ShopDataPath)) 
                return;

            ClearShopUI();
            string lJson = File.ReadAllText(_ShopDataPath);
            ShopData lData = JsonConvert.DeserializeObject<ShopData>(lJson);

            _AvailableItems.Clear();

            if (lData == null || lData.cardsNamesInShop == null) 
                return;
            
            foreach (string lName in lData.cardsNamesInShop)
            {
                CardCharacterSO lCard = _DeckBuilderManager.allCards.Find(pX => pX.CardName == lName);
                if (lCard != null) _AvailableItems.Add(lCard);
            }
            
            DisplayShop();
        }
        
        private void ClearShopUI()
        {
            foreach (Transform lSlot in _ShopSlots)
                foreach (Transform lChild in lSlot) 
                    Destroy(lChild.gameObject);
        }
        
        private void OnDestroy()
        {
            if (_GameManager != null)
                _GameManager.onMidNight -= RefreshShop;
        }
    }
}