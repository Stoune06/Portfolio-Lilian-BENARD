using System;
using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.SO;
using Com.IsartDigital.HealerSurvivor.Other;
using UnityEngine;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 13/04/2026 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.UI
{
    [RequireComponent(typeof(Canvas))]
    public class CardDisplay : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [Header("UI Components")]
        [SerializeField] private Text _NameText;
        [SerializeField] private Image _CardIcon;
        [SerializeField] private Image _CardBackground;
        
        [Header("Interactions")]
        [SerializeField] private Button _CardImageButton;
        [SerializeField] private Button _CloseZoneButton;
        [SerializeField] private Description _CardDescription;

        private CardCharacterSO _CardData;
        private Transform _OriginalParent;
        private Transform _DescriptionContainer;
        private Color _CardColorRarity;
        private ERarity _CardRarity;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            _OriginalParent = _CardDescription.transform.parent;
            
            transform.SetAsFirstSibling();
            _CardDescription.gameObject.SetActive(false);
            _CloseZoneButton.gameObject.SetActive(false);

            _CardImageButton.onClick.AddListener(() => ButtonAction(_CardDescription.Show, true));
            _CloseZoneButton.onClick.AddListener(() => ButtonAction(_CardDescription.Hide, false));

            _CardDescription.onHide += () => _CloseZoneButton.gameObject.SetActive(false);
        }

        public void SetCard(CardCharacterSO pData, Transform pDescriptionContainer = null)
        {
            _CardData = pData;
            _DescriptionContainer = pDescriptionContainer;

            _NameText.text = pData.CardName;
            _CardIcon.sprite = pData.CardIcon;
            _CardRarity = pData.Rarity;
            
            _CardDescription.SetDescriptionSprite(pData.CardDescription);

            _CardColorRarity = RaritySettings.GetColor(_CardRarity);
            _CardBackground.color = _CardColorRarity;
        }

        public CardCharacterSO GetCardData() => _CardData;
        
        private void ButtonAction(Action pActionToDo, bool pShow)
        {
            pActionToDo?.Invoke();
            _CloseZoneButton.gameObject.SetActive(pShow);
            
            if (pShow)
            {
                _CardDescription.transform.SetParent(_DescriptionContainer, false);
                _CloseZoneButton.transform.SetParent(_DescriptionContainer, false);
                _CloseZoneButton.transform.SetAsFirstSibling();
                _CardDescription.transform.SetAsLastSibling();
        
            }
            else
            {
                _CardDescription.transform.SetParent(_OriginalParent);
                _CloseZoneButton.transform.SetParent(_OriginalParent);
            }
        }

        private void OnDestroy()
        {
            _CardImageButton.onClick.RemoveAllListeners();
            _CloseZoneButton.onClick.RemoveAllListeners();
            
            if (_CardDescription != null) 
                _CardDescription.onHide -= () => _CloseZoneButton.gameObject.SetActive(false);
        }
    }
}