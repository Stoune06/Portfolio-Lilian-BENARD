using System;
using System.Collections;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Manager;
using Com.IsartDigital.HealerSurvivor.SO;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.UI
{
    
    public class PlayMenu : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private Transform _CardContainer;
        [SerializeField] private GameObject _CardPrefab;
        
        private DeckBuilderManager _DeckBuilderManager => DeckBuilderManager.Instance;
        private GameManager _GameManager => GameManager.Instance;
        
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            RefreshDisplay();
            _GameManager.onSwitchCardFromDeck += RefreshDisplay;
        }
        
        private void RefreshDisplay()
        {
            foreach (Transform child in _CardContainer)
                Destroy(child.gameObject);
            
            if (_DeckBuilderManager == null) 
                return;

            foreach (CardCharacterSO lCard in _DeckBuilderManager.activeCurrentCards)
                InstantiateCard(lCard, _CardContainer);
        }

        private void InstantiateCard(CardCharacterSO pData, Transform pParent)
        {
            GameObject lGo = Instantiate(_CardPrefab, pParent);
            CardDisplay lDisplay = lGo.GetComponent<CardDisplay>();

            lDisplay.SetCard(pData);
        }

        private void OnEnable()
        {
            RefreshDisplay();
            
            if (_GameManager != null)
                _GameManager.onSwitchCardFromDeck += RefreshDisplay;
        }
        
        private void OnDisable()
        {
            if (_GameManager != null)
                _GameManager.onSwitchCardFromDeck -= RefreshDisplay;
        }
    }
}