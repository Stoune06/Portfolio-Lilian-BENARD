using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Manager;
using DG.Tweening;
using TMPro;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.UI
{
    
    public class HUD : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private TextMeshProUGUI _CurrentCoinText;
        
        private Tween _Tween;

        private const float TWEEN_VALUE = .2f;

        private GameManager _GameManager => GameManager.Instance;
        private CurrencyManager _CurrencyManager => CurrencyManager.Instance;
        
        private void RequestAddCoins() => _CurrencyManager.AddCurrency(1, ECurrencyType.GOLD);
        
        private void UpdateCoinText(ECurrencyType pType, int pNewAmount)
        {
            if (pType != ECurrencyType.GOLD) 
                return;
            
            _CurrentCoinText.text = "x " + pNewAmount.ToString();
                
            _Tween?.Kill();
            _CurrentCoinText.transform.localScale = Vector3.one;

            _Tween = _CurrentCoinText.transform.DOPunchScale(Vector3.one * TWEEN_VALUE, TWEEN_VALUE)
                .OnStart(() => _CurrentCoinText.color = Color.yellow) 
                .OnComplete(() => _CurrentCoinText.color = Color.white);
        }

        private void OnEnable()
        {
            if (_CurrencyManager != null)
                _CurrencyManager.OnCurrencyChanged += UpdateCoinText;
            
            if (GameManager.Instance != null)
                _GameManager.onGainingCoins += RequestAddCoins;
        }

        private void OnDisable()
        {
            if (_CurrencyManager != null)
                _CurrencyManager.OnCurrencyChanged -= UpdateCoinText;
            
            if (GameManager.Instance != null)
                _GameManager.onGainingCoins -= RequestAddCoins;
        }
    }
}