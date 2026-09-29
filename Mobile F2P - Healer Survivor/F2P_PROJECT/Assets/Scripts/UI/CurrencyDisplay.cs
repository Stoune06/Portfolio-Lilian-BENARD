using System;
using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Manager;
using TMPro;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.UI
{
    
    public class CurrencyDisplay : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private TextMeshProUGUI _CurrentCoinText;
        [SerializeField] private TextMeshProUGUI _CurrentGemText;
        
        private CurrencyManager _CurrencyManager => CurrencyManager.Instance;

        private void Start()
        {
            if (_CurrencyManager == null) 
                return;
            
            _CurrencyManager.OnCurrencyChanged += UpdateDisplay;
            
            UpdateDisplay(ECurrencyType.GOLD, _CurrencyManager.Gold);
            UpdateDisplay(ECurrencyType.GEMS, _CurrencyManager.Gems);
        }

        private void OnEnable()
        {
            if (_CurrencyManager == null) 
                return;
            
            _CurrencyManager.OnCurrencyChanged += UpdateDisplay;
            
            UpdateDisplay(ECurrencyType.GOLD, _CurrencyManager.Gold);
            UpdateDisplay(ECurrencyType.GEMS, _CurrencyManager.Gems);
        }

        private void OnDisable()
        {
            if (_CurrencyManager != null)
                _CurrencyManager.OnCurrencyChanged -= UpdateDisplay;
        }

        private void UpdateDisplay(ECurrencyType pType, int pValue)
        {
            switch (pType)
            {
                case ECurrencyType.GOLD:
                    _CurrentCoinText.text = "x " + pValue.ToString();
                    break;
                case ECurrencyType.GEMS:
                    _CurrentGemText.text = "x " + pValue.ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pType), pType, null);
            }
        }
    }
}