using Com.IsartDigital.HealerSurvivor.Other;
using Com.IsartDigital.HealerSurvivor.SO;
using System;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 13/04/2026 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Manager
{
    
    public class DeckBuilderManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [NonSerialized] public List<CardCharacterSO> allCards = new List<CardCharacterSO>();
        [NonSerialized] public List<CardCharacterSO> activeCurrentCards = new List<CardCharacterSO>();

        private const int MAX_CARD_NUMBER = 4;

        public static DeckBuilderManager Instance { get; private set; }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            #region SINGLETON
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            #endregion

            FindAllCards();
        }

        private void FindAllCards()
        {
            allCards.Clear();
            CardCharacterSO[] lLoadedCards = Resources.LoadAll<CardCharacterSO>(Utils.CHARACTER_CARD_SO_FILE_REF);
            allCards.AddRange(lLoadedCards);
        }

        public void AddCardToDeck(CardCharacterSO pCurrentCard)
        {
            if (activeCurrentCards.Count < MAX_CARD_NUMBER && !activeCurrentCards.Contains(pCurrentCard))
                activeCurrentCards.Add(pCurrentCard);
        }

        public void RemoveCardFromDeck(CardCharacterSO pCurrentCard) => activeCurrentCards.Remove(pCurrentCard);
        
    }
}